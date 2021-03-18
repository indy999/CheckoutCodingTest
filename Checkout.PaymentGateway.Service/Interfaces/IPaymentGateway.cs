namespace Checkout.PaymentGateway.Service.Interfaces
{
    using Checkout.PaymentGateway.Models;
    using System;
    using System.Threading.Tasks;

    public interface IPaymentGateway
    {
        Task<PaymentResponse> MakePaymentAsync(PaymentRequest paymentRequest);
        Task<PaymentDetail> GetPaymentDetailAsync(Guid paymentId);
    }
}
