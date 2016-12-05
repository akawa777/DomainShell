using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Domain
{
    public class CartValidationSpec : IValidationSpec<CartEntity>
    {
        public CartValidationSpec(IProductReadService productReadService)
        {
            _productReadService = productReadService;
        }

        private IProductReadService _productReadService;

        public bool Validate(CartEntity target, out string[] errors)
        {
            List<string> errorList = new List<string>();
            foreach (CartItemEntity item in target.CartItemList)
            {
                if (_productReadService.Find(item.ProductId) == null)
                {
                    errorList.Add(string.Format("CartItemNo {0} Product not exist", item.Id.CartItemNo));
                }

                if (item.Quantity < 0)
                {
                    errorList.Add(string.Format("Quantity in valid", item.Id.CartItemNo));
                }
            }

            errors = errorList.ToArray();

            return errors.Length == 0;
        }
    }
}
