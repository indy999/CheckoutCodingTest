namespace Checkout.PaymentGateway.Service.Interfaces
{
    using Checkout.PaymentGateway.Models;
    using System;
    using System.Threading.Tasks;

    public interface IRepository
    {
        void SavePaymentDetailAsync(PaymentDetail paymentDetail);
        Task<PaymentDetail> GetPaymentDetailAsync(Guid id);
    }
}
