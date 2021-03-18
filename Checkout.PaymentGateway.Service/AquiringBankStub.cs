namespace Checkout.PaymentGateway.Service
{
    using Checkout.PaymentGateway.Models;
    using Checkout.PaymentGateway.Service.Interfaces;
    using System;
    using System.Threading.Tasks;

    public class AquiringBankStub : IBank
    {
        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            if (paymentRequest == null)
            {
                return null;
            }

            var response = new PaymentResponse { Id = Guid.NewGuid() };

            if (string.IsNullOrEmpty(paymentRequest.CVV) ||
                string.IsNullOrEmpty(paymentRequest.CardNumber) ||
                string.IsNullOrEmpty(paymentRequest.CardHolderName) ||
                string.IsNullOrEmpty(paymentRequest.CardHolderName) ||
                string.IsNullOrEmpty(paymentRequest.Currency) ||
                string.IsNullOrEmpty(paymentRequest.ExpiryDate) ||
                paymentRequest.Amount <= 0 ||
                paymentRequest.CardNumber.Length != 16 ||
                paymentRequest.Currency.Length != 3)
            {
                response.Status = Models.Enums.TransactionStatus.Declined;
                return response;
            }

            var expiryDateSplit = paymentRequest.ExpiryDate.Split("/");
            if (expiryDateSplit.Length != 2)
            {
                response.Status = Models.Enums.TransactionStatus.Declined;
                return response;
            }

            if (!int.TryParse(expiryDateSplit[0], out _))
            {
                response.Status = Models.Enums.TransactionStatus.Declined;
                return response;
            }

            int.TryParse(expiryDateSplit[0], out var month);

            if (month > 12 || month < 1)
            {
                response.Status = Models.Enums.TransactionStatus.Declined;
                return response;
            }

            if (!int.TryParse(expiryDateSplit[1], out var year))
            {
                response.Status = Models.Enums.TransactionStatus.Declined;
                return response;
            }

            var expiryDate = new DateTime(year + 2000, month, 1).AddMonths(1).AddDays(-1);
            if (DateTime.Today > expiryDate.Date)
            {
                response.Status = Models.Enums.TransactionStatus.Declined;
                return response;
            }

            return response;
        }        
    }
}
