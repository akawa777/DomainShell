using Microsoft.VisualStudio.TestTools.UnitTesting;
using FreestyleOrm;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace FreeStyleOrm.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethodByManual()
        {
            IDbConnection connection = null;

            IQuery<PurchaseOrder> query = connection.Query<PurchaseOrder>(
                @"
                    select
                        po.*,

                        c.CustomerName,
                        c.RecordVersion c_RecordVersion

                        pi.PurchaseItemNo,
                        pi.RecordVersion pi_RecordVersion

                        p.ProductName,
                        p.RecordVersion p_RecordVersion
                    from
                        PurchaseOrder po
                    left join
                        Customer c
                    on
                        po.CustomerId = c.CustomerId
                    left join
                        PurdfhaeItem pi
                    on
                        po.PurchaseOrderId = pi.PurchaseOrderId
                    left join
                        Product p
                    on
                        po.ProductId = p.ProductId
                    order by
                        po.PurchaseOrderId,
                        pi.PurchaseItemNo
                ");

            query.Map(m =>
            {
                m.To()
                    .UniqueKeys("PurchaseOrderId")            
                    .CreateEntity(() => new PurchaseOrder())
                    .FormatPropertyName(column => column)                    
                    .Refer(Refer.Write)
                    .SetEntity((row, entity) =>
                    {                        
                        entity.PurchaseOrderId = (int)row[nameof(entity.PurchaseOrderId)];
                    })
                    .SetRow((entity, rootNode, row) =>
                    {
                        row[nameof(entity.PurchaseOrderId)] = entity.PurchaseOrderId;
                    })
                    .Table(t =>
                    {
                        t.Name("PurchaseOrder");
                    })
                    .AutoId(true)
                    .OptimisticLock("RecordVersion", entity => entity.RecordVersion + 1);                    

                m.ToOne(rootEntity => rootEntity.Customer)
                    .UniqueKeys("CustomerId")
                    .IncludePrefix("c");

                m.ToMany(rootEntity => rootEntity.PurchaseItems)
                    .UniqueKeys("PurchaseOrderId, PurchaseItemNo")
                    .CreateEntity(() => new PurchaseItem())
                    .FormatPropertyName(column => column)
                    .IncludePrefix("pi")
                    .Refer(Refer.Write)
                    .SetEntity((row, entity) =>
                    {
                        entity.PurchaseItemNo = (int)row[nameof(entity.PurchaseItemNo)];
                    })
                    .SetRow((entity, rootNode, row) =>
                    {
                        PurchaseOrder order = (PurchaseOrder)rootNode.Entity;

                        row[nameof(order.PurchaseOrderId)] = order.PurchaseOrderId;
                        row[nameof(entity.PurchaseItemNo)] = entity.PurchaseItemNo;
                    })
                    .Table(t =>
                    {
                        t.Name("PurdfhaeItem");
                        t.RelationId("PurchaseOrderId", x => x);
                    })
                    .OptimisticLock("RecordVersion", entity => entity.RecordVersion + 1);

                m.ToOne(rootEntity => rootEntity.PurchaseItems.First())
                    .UniqueKeys("ProductId")
                    .IncludePrefix("pi");
            });
        }
   
        [TestMethod]
        public void TestMethodByAuto()
        {
            IQuery<PurchaseOrder> query = null;

            query.Map(m =>
            {
                m.To()
                    .UniqueKeys("PurchaseOrderId")
                    .Refer(Refer.Write)
                    .AutoId(true)
                    .OptimisticLock("RecordVersion", entity => entity.RecordVersion + 1);

                m.ToOne(rootEntity => rootEntity.Customer)
                    .UniqueKeys("CustomerId")
                    .IncludePrefix("c");

                m.ToMany(rootEntity => rootEntity.PurchaseItems)
                    .UniqueKeys("PurchaseOrderId, PurchaseItemNo")
                    .IncludePrefix("pi")
                    .Refer(Refer.Write)
                    .Table(t => t.RelationId("PurchaseOrderId", x => x))
                    .OptimisticLock("RecordVersion", entity => entity.RecordVersion + 1);

                m.ToOne(rootEntity => rootEntity.PurchaseItems.First())
                    .UniqueKeys("ProductId")
                    .IncludePrefix("pi");
            });
        }

        public class PurchaseOrder
        {
            public int PurchaseOrderId { get; set; }
            public Customer Customer { get; set; }
            public IEnumerable<PurchaseItem> PurchaseItems { get; set; }
            public int RecordVersion { get; set; }
        }

        public class Customer
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int RecordVersion { get; set; }
        }

        public class PurchaseItem
        {
            public int PurchaseItemNo { get; set;}
            public Product Product { get; set; }
            public int RecordVersion { get; set; }
        }

        public class Product
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int RecordVersion { get; set; }
        }

    }
}
