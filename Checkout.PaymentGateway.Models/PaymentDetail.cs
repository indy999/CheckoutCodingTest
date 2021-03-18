namespace Checkout.PaymentGateway.Models
{
    using System;
    public class PaymentDetail
    {
        public PaymentDetail(PaymentRequest request, PaymentResponse response)
        {
            Request = request;
            Response = response;

            if (response != null)
                Id = response.Id;
        }

        public Guid Id { get; set; }
        public PaymentRequest Request { get; set; }
        public PaymentResponse Response { get; set; }
    }
}