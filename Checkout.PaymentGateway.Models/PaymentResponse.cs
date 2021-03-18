namespace Checkout.PaymentGateway.Models
{
    using Checkout.PaymentGateway.Models.Enums;
    using System;

    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public TransactionStatus Status { get; set; }
    }
}