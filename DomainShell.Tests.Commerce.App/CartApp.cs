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
    public class CartItemListRequest
    {
        public int CustomerId { get; set; }
    }

    public class CartItemListResult
    {
        public int CustomerId { get; set; }
        public int CartNo { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CartItemAddRequest
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemRemoveRequest
    {
        public int CustomerId { get; set; }
        public int CartItemNo { get; set; }
    }

    public class CartPurchaseRequest
    {
        public int CustomerId { get; set; }
        public int CardCompanyId { get; set; }
        public int CardNo { get; set; }
    }

    public class CartApp : 
        IApp<CartItemListRequest, IEnumerable<CartItemListResult>>,
        IApp<CartItemAddRequest>,
        IApp<CartItemRemoveRequest>,
        IApp<CartPurchaseRequest>
    {
        public CartApp(ISession session)
        {
            _session = session;

            DomainEventDispatcher domainEventDispatcher = new DomainEventDispatcher();

            _cartFactory = new Infrastructure.Factories.CartFactory(_session);
            _cartRepository = new Infrastructure.Repositories.CartRepository(_session, domainEventDispatcher);
            _purchaseFactory = new Infrastructure.Factories.PurchaseFactory(_session);
            _purchaseRepository = new Infrastructure.Repositories.PurchaseRepository(_session, domainEventDispatcher);
            _creditCardService = new Infrastructure.Services.CreditCardService();
            _productReadService = new Infrastructure.Services.ProductReadService(_session);
            _cartReader = new Infrastructure.Services.CartReader(_session);

            domainEventDispatcher.Register<CartPurchasedEvent>(new CartEventHandler(_purchaseFactory, _purchaseRepository));
        }

        private ISession _session;
        private ICartFactory _cartFactory;
        private ICartRepository _cartRepository;
        private IPurchaseFactory _purchaseFactory;
        private IPurchaseRepository _purchaseRepository;
        private ICreditCardService _creditCardService;
        private IProductReadService _productReadService;
        private ICartReader _cartReader;

        public IEnumerable<CartItemListResult> Execute(CartItemListRequest request)
        {
            foreach (CartItemReadDto dto in _cartReader.GetCartItemList(request.CustomerId))
            {
                yield return new CartItemListResult
                {
                    CustomerId = dto.CustomerId,
                    CartNo = dto.CartNo,
                    ProductId = dto.ProductId,
                    ProductName = dto.ProductName,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    TotalPrice = dto.TotalPrice
                };
            }
        }

        public void Execute(CartItemAddRequest request)
        {
            using (ITran tran = _session.Tran())
            {
                CartEntity cart = _cartRepository.Find(new CartId(request.CustomerId));

                if (cart == null)
                {
                    CartCreationSpec spec = new CartCreationSpec(request.CustomerId);
                    cart = _cartFactory.Create(spec);
                }

                cart.AddProduct(request.ProductId, request.Quantity);

                _cartRepository.Save(cart);

                tran.Complete();
            }
        }

        public void Execute(CartItemRemoveRequest request)
        {
            using (ITran tran = _session.Tran())
            {
                CartEntity cart = _cartRepository.Find(new CartId(request.CustomerId));

                cart.RemoveProduct(new CartDetailId(request.CustomerId, request.CartItemNo));

                _cartRepository.Save(cart);

                tran.Complete();
            }
        }

        public void Execute(CartPurchaseRequest request)
        {
            using (ITran tran = _session.Tran())
            {
                CartEntity cart = _cartRepository.Find(new CartId(request.CustomerId));

                cart.Purchase(new CreditCardValue(request.CardCompanyId, request.CardNo), _creditCardService, _productReadService);

                _cartRepository.Save(cart);

                tran.Complete();
            }
        }
    }
}
