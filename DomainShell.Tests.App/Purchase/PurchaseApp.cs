using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Mappers;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Purchase;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Infrastructure.Common;
using DomainShell.Tests.Infrastructure.Cart;
using DomainShell.Tests.Infrastructure.Customer;
using DomainShell.Tests.Infrastructure.Product;
using DomainShell.Tests.Infrastructure.Purchase;

namespace DomainShell.Tests.App.Purchase
{
    public class PurchaseApp
    {
        public PurchaseApp()
        {
            _session = new Session(new SqliteSessionKernel());
            
            _purchasetReader = new PurchasetReader(_session);

            _mapperConfig = new MapperConfiguration(configExpression =>
            {
                configExpression.CreateMap<PurchaseReadModel, Purchase>();
            });

            _mapper = new Mapper(_mapperConfig);
        }

        private Session _session;        
        private PurchasetReader _purchasetReader;
        private MapperConfiguration _mapperConfig;
        private IMapper _mapper;

        public Purchase[] GetPurchases(PurchasesQuery query)
        {
            using (_session.Connect())
            {
                PurchaseReadModel[] readModels = _purchasetReader.GetPurchases(query.CustomerId);

                return _mapper.Map<Purchase[]>(readModels);
            }
        }
    }
}
