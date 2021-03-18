namespace Checkout.PaymentGateway.Service
{
    using Checkout.PaymentGateway.Models;
    using Checkout.PaymentGateway.Service.Interfaces;
    using System;
    using System.Threading.Tasks;

    public class PaymentGatewayService : IPaymentGateway
    {
        private readonly IBank _acquiringBankService;
        private readonly IRepository _repository;

        public PaymentGatewayService(IBank acquiringBankService, IRepository repository)
        {
            _acquiringBankService = acquiringBankService;
            _repository = repository;
        }

        public async Task<PaymentDetail> GetPaymentDetailAsync(Guid paymentId)
        {
            var paymentDetail = await _repository.GetPaymentDetailAsync(paymentId);

            if (paymentDetail != null)
            {
                paymentDetail.Request.CardNumber = paymentDetail.Request.MaskCardNumber();
            }

            return paymentDetail;
        }

        public async Task<PaymentResponse> MakePaymentAsync(PaymentRequest paymentRequest)
        {
            var paymentResponse =  await _acquiringBankService.ProcessPaymentAsync(paymentRequest);

            var paymentDetail = new PaymentDetail(paymentRequest, paymentResponse);
            _repository.SavePaymentDetailAsync(paymentDetail);

            return paymentResponse;
        }
    }
}
