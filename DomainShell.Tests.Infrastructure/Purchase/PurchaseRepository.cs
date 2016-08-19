using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Purchase;

namespace DomainShell.Tests.Infrastructure.Purchase
{
    public class PurchaseRepository : IRepositroy<PurchaseModel>
    {
        public PurchaseRepository(Session session)
        {
            _session = session;
        }

        private Session _session;

        public void Save(PurchaseModel purchase)
        {
            if (purchase.State.GetState() == State.StateFlg.New)
            {
                Create(purchase);
            }

            purchase.State.UnChanged();
        }

        private void Create(PurchaseModel purchase)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                insert into Payment (PaymentDate, CustomerId, CreditCardNo, CreditCardHolder, CreditCardExpirationDate, ShippingAddress, Postage, Tax, PaymentAmount) 
                values (@PaymentDate, @CustomerId, @CreditCardNo, @CreditCardHolder, @CreditCardExpirationDate, @ShippingAddress, @Postage, @Tax, @PaymentAmount) 
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@PaymentDate";
            param.Value = purchase.PaymentDate;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CustomerId";
            param.Value = purchase.CustomerId;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CreditCardNo";
            param.Value = purchase.CreditCard.CreditCardNo;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CreditCardHolder";
            param.Value = purchase.CreditCard.CreditCardHolder;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CreditCardExpirationDate";
            param.Value = purchase.CreditCard.CreditCardExpirationDate;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@ShippingAddress";
            param.Value = purchase.ShippingAddress;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@Postage";
            param.Value = purchase.Postage;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@Tax";
            param.Value = purchase.Tax;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@PaymentAmount";
            param.Value = purchase.PaymentAmount;
            command.Parameters.Add(param);

            command.ExecuteNonQuery();

            command.CommandText = @"
                select PaymentId from Payment where ROWID = last_insert_rowid();         
            ";

            var id = command.ExecuteScalar();

            purchase.PurchaseId = id.ToString();
            
            foreach (PurchaseDetailModel detail in purchase.PurchaseDetailList)
            {
                command.CommandText = @"
                    insert into PaymentItem (PaymentId, PaymentItemId, ProductId, PriceAtTime, Number) values (@PaymentId, @PaymentItemId, @ProductId, @PriceAtTime, @Number) 
                ";

                param = command.CreateParameter();
                param.ParameterName = "@PurchaseId";
                param.Value = purchase.PurchaseId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@PurchaseDetailId";
                param.Value = detail.PurchaseDetailId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ProductId";
                param.Value = detail.ProductId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@PriceAtTime";
                param.Value = detail.PriceAtTime;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@Number";
                param.Value = detail.Number;
                command.Parameters.Add(param);

                command.ExecuteNonQuery();
            }
        }
    }
}
