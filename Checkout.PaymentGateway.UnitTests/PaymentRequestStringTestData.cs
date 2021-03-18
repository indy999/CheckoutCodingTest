namespace Checkout.PaymentGateway.UnitTests
{
    using Checkout.PaymentGateway.Models;
    using Checkout.PaymentGateway.Models.Enums;
    using System.Collections;
    using System.Collections.Generic;
    public class PaymentRequestStringTestData : IEnumerable<object[]>
    {
        private const string Cvv = "123";
        private const string CardHolderName = "A.Smith";
        private const string CardNumber = "1234456712349999";
        private const string Currency = "GBP";
        private const double Amount = 10.00;
        private const string ExpiryDate = "02/25";

        private readonly List<object[]> _data = new List<object[]>
        {
            CreateTestPaymentRequestAndExpectedStatus(null,CardHolderName,CardNumber,Currency,Amount,ExpiryDate,TransactionStatus.Declined),
            CreateTestPaymentRequestAndExpectedStatus(Cvv,null,CardNumber,Currency,Amount,ExpiryDate,TransactionStatus.Declined),
            CreateTestPaymentRequestAndExpectedStatus(Cvv,CardHolderName,null,Currency,Amount,ExpiryDate,TransactionStatus.Declined),
            CreateTestPaymentRequestAndExpectedStatus(Cvv,CardHolderName,CardNumber,null,Amount,ExpiryDate,TransactionStatus.Declined),
            CreateTestPaymentRequestAndExpectedStatus(Cvv,CardHolderName,CardNumber,Currency,-1,ExpiryDate,TransactionStatus.Declined),
            CreateTestPaymentRequestAndExpectedStatus(Cvv,CardHolderName,CardNumber,Currency,Amount,null,TransactionStatus.Declined),
        };

        private static object[] CreateTestPaymentRequestAndExpectedStatus(string cvv,
                                string cardHolderName, string cardNumber, string currency,
                                double amount, string expiryDate, TransactionStatus expectedStatus)
        {
            return new object[] { new PaymentRequest
            {
                CVV = cvv,
                CardHolderName = cardHolderName,
                CardNumber = cardNumber,
                Currency = currency,
                Amount = amount,
                ExpiryDate = expiryDate
            },
            expectedStatus
            };
        }

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

   
