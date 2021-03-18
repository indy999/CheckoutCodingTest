namespace Checkout.PaymentGateway.Api.Controllers
{
    using Checkout.PaymentGateway.Api.Attributes;
    using Checkout.PaymentGateway.Models;
    using Checkout.PaymentGateway.Service.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPaymentGateway paymentGateway, ILogger<PaymentController> logger)
        {
            this._paymentGateway = paymentGateway;
            _logger = logger;
        }

        [HttpPost]
        [Route("/api/payment/makepayment")]
        public async Task<IActionResult> MakePayment([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                if (ValidatePaymentRequest(paymentRequest))
                {
                    var result = await _paymentGateway.MakePaymentAsync(paymentRequest);

                    return StatusCode(201, result);
                }

                return StatusCode(422);
            }
            catch(Exception e)
            {
                _logger.LogError("Error Processing Payment", e);
                return StatusCode(502);
            }
           
        }

        [HttpGet]
        [Route("/api/payment/getpaymentdetail/{paymentId}")]
        public async Task<IActionResult> GetPaymentDetail(Guid paymentId)
        {
            var response = await _paymentGateway.GetPaymentDetailAsync(paymentId);

            if(response == null)
            {
                return StatusCode(404);
            }

            return StatusCode(201, response);
        }
        private bool ValidatePaymentRequest(PaymentRequest paymentRequest)
        {
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
                return false;
            }

            return true;
        }
    }
}
