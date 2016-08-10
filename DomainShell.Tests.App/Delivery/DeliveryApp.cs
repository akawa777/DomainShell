using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Infrastructure.Delivery;
using DomainShell.Tests.Domain.Delivery;

namespace DomainShell.Tests.App.Delivery
{
    public class DeliveryData
    {
        public string PaymentId { get; set; }
        public string DeliveryId { get; set; }        
        public string CustomerName { get; set; }
        public string TopProductName { get; set; }
        public string BeginDate { get; set; }
        public string CompleteDate { get; set; }
    }  

    public class DeliveryApp
    {
        public DeliveryApp()
        {
            _session = new Session();

            _deliveryRepository = new DeliveryRepository(_session);
            _deliveryReader = new DeliveryReader(_session);
        }

        private Session _session;
        private DeliveryRepository _deliveryRepository;
        private DeliveryReader _deliveryReader;

        public void Send(DeliveryData data)
        {            
            using (Transaction tran = _session.BegingTran())
            {
                DeliveryModel delivery = _deliveryRepository.Get(data.PaymentId, data.DeliveryId);

                delivery.Send();

                _deliveryRepository.Save(delivery);

                tran.Commit();
            }            
        }

        public void Complete(DeliveryData data)
        {
            using (Transaction tran = _session.BegingTran())
            {
                DeliveryModel delivery = _deliveryRepository.Get(data.PaymentId, data.DeliveryId);
                delivery.Complete();

                _deliveryRepository.Save(delivery);

                tran.Commit();
            }       
        }

        public DeliveryData[] GetAll()
        {
            using (_session.Open())
            {
                List<DeliveryReadModel> deliveries = _deliveryReader.GetAllDelivery();

                return deliveries.Select(x =>
                    new DeliveryData 
                    { 
                        PaymentId = x.PaymentId,
                        DeliveryId = x.DeliveryId,
                        CustomerName = x.CustomerName,
                        TopProductName = x.TopProductName,
                        BeginDate = x.BeginDate,
                        CompleteDate = x.CompleteDate
                    }
                ).ToArray();
            }            
        }
    }
}
