namespace Checkout.PaymentGateway.Service
{
    using Checkout.PaymentGateway.Models;
    using Checkout.PaymentGateway.Service.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class InMemoryStorage : IRepository
    {
        private readonly Dictionary<Guid, object> _storageObject = new Dictionary<Guid, object>();

        public void SavePaymentDetailAsync(PaymentDetail paymentDetail)
        {
            if (_storageObject.ContainsKey(paymentDetail.Response.Id))
            {
                _storageObject[paymentDetail.Response.Id] = paymentDetail;
            }
            else
            {
                _storageObject.Add(paymentDetail.Response.Id, paymentDetail);
            }
        }

        public async Task<PaymentDetail> GetPaymentDetailAsync(Guid id)
        {
            if (!_storageObject.ContainsKey(id))
            {
                return null;
            }

            return _storageObject[id] as PaymentDetail;
        }
    }
}
