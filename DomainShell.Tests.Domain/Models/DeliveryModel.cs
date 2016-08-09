using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Models
{
    public class DeliveryModel : IAggregateRoot
    {
        public DeliveryModel()
        {

        }

        public DeliveryModel(PaymentModel payment)
            : this()
        {
            Payment = payment;
        }

        public string DeliveryId { get; set; }
        public PaymentModel Payment { get; private set; }
        public string CompleteTime { get; set; }

        public void Send(PaymentModel payment)
        {
            if (Payment != null)
            {
                throw new Exception("already send.");
            }

            Payment = payment;

            State = State.Created;
        }

        public void Complete()
        {
            if (!string.IsNullOrEmpty(CompleteTime))
            {
                throw new Exception("already complete.");
            }

            CompleteTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public State State { get; private set; }

        public void Accepted()
        {
            State = State.UnChanged;
        }
    }   
}
