namespace Checkout.PaymentGateway.Service.Interfaces
{
    using Checkout.PaymentGateway.Models;
    using System.Threading.Tasks;
   
    public interface IBank  
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest paymentRequest);
    }
}
