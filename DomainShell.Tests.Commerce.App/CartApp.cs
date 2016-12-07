using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.App;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;
using DomainShell.Tests.Commerce.Domain.Handlers;
using DomainShell.Tests.Commerce.Infrastructure;
using DomainShell.Tests.Commerce.Infrastructure.Contracts;

namespace DomainShell.Tests.Commerce.App
{
    public interface ICartApp :
        IApp<CartItemListRequest, CartItemResponse[]>,
        IApp<CartItemAddRequest>,
        IApp<CartItemRemoveRequest>,
        IApp<CartPurchaseRequest>
    {

    }

    public class CartApp : ICartApp        
    {
        public CartApp(ISession session)
        {
            _session = session;

            DomainEventDispatcher domainEventDispatcher = new DomainEventDispatcher();

            _cartFactory = new Infrastructure.Factories.CartFactory(_session);
            _cartRepository = new Infrastructure.Repositories.CartRepository(_session, domainEventDispatcher);
            
            _productReadService = new Domain.Services.ProductReadService(new Infrastructure.Repositories.ProductRepository(_session));
            _cartReadService = new Infrastructure.Services.CartReadService(_session);

            ICreditCardService creditCardService = new Infrastructure.Services.CreditCardService();
            IPurchaseFactory purchaseFactory = new Infrastructure.Factories.PurchaseFactory(_session);
            IPurchaseRepository purchaseRepository = new Infrastructure.Repositories.PurchaseRepository(_session, domainEventDispatcher);

            domainEventDispatcher.Register<CartPurchasedEvent>(new CartEventHandler(creditCardService, purchaseFactory, purchaseRepository));
        }

        private ISession _session;
        private ICartFactory _cartFactory;
        private ICartRepository _cartRepository;        
        private IProductReadService _productReadService;
        private ICartReadService _cartReadService;

        public CartItemResponse[] Execute(CartItemListRequest request)
        {
            List<CartItemResponse> list = new List<CartItemResponse>();
            foreach (CartItemReadDto dto in _cartReadService.GetCartItemList(request.CustomerId))
            {
                list.Add(new CartItemResponse
                {
                    CustomerId = dto.CustomerId,
                    CartNo = dto.CartNo,
                    ProductId = dto.ProductId,
                    ProductName = dto.ProductName,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    TotalPrice = dto.TotalPrice
                });
            }

            return list.ToArray();
        }

        public void Execute(CartItemAddRequest request)
        {
            using (ITran tran = _session.Tran())
            {
                CartEntity cart = _cartRepository.Find(new CartId(request.CustomerId));

                if (cart == null)
                {
                    CartCreationSpec creationSpec = new CartCreationSpec(request.CustomerId);
                    cart = _cartFactory.Create(creationSpec);
                }

                cart.AddProduct(request.ProductId, request.Quantity);

                CartValidationSpec validationSpec = new CartValidationSpec(_productReadService);

                cart.Validate(validationSpec);

                _cartRepository.Save(cart);

                tran.Complete();
            }
        }

        public void Execute(CartItemRemoveRequest request)
        {
            using (ITran tran = _session.Tran())
            {
                CartEntity cart = _cartRepository.Find(new CartId(request.CustomerId));

                cart.RemoveProduct(new CartItemId(request.CustomerId, request.CartItemNo));

                _cartRepository.Save(cart);

                tran.Complete();
            }
        }

        public void Execute(CartPurchaseRequest request)
        {
            using (ITran tran = _session.Tran())
            {
                CartEntity cart = _cartRepository.Find(new CartId(request.CustomerId));

                CreditCardValue creditCard = new CreditCardValue(request.CardCompanyId, request.CardNo);
                CartValidationSpec validationSpec = new CartValidationSpec(_productReadService);

                cart.Purchase(creditCard, _productReadService, validationSpec);

                _cartRepository.Save(cart);

                tran.Complete();
            }
        }
    }
}
