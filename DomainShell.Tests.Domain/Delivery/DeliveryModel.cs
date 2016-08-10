using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Payment;

namespace DomainShell.Tests.Domain.Delivery
{
    public class DeliveryModel : IAggregateRoot
    {
        public string PaymentId { get; set; }
        public string DeliveryId { get; set; }
        public string BeginDate { get; set; }        
        public string CompleteDate { get; set; }

        public void Send()
        {
            if (!string.IsNullOrEmpty(BeginDate))
            {
                throw new Exception("already send.");
            }

            BeginDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            State = State.Updated;
        }

        public void Complete()
        {
            if (!string.IsNullOrEmpty(CompleteDate))
            {
                throw new Exception("already complete.");
            }

            CompleteDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            State = State.Updated;
        }

        public State State { get; private set; }

        public void Accepted()
        {
            State = State.UnChanged;
        }
    }   
}
