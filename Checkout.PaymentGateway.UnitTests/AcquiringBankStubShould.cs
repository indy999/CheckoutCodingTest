namespace Checkout.PaymentGateway.UnitTests
{
    using Checkout.PaymentGateway.Models;
    using Checkout.PaymentGateway.Models.Enums;
    using Checkout.PaymentGateway.Service;
    using FluentAssertions;
    using System;
    using Xunit;

    public class AcquiringBankStubShould
    {
        private AquiringBankStub acquiringBankStub;

        public AcquiringBankStubShould()
        {
            acquiringBankStub = new AquiringBankStub();
        }

        [Fact]
        public void ProcessPaymentSuccessfully()
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1111222233334444",
                CardHolderName = "A.Smith",
                CVV = "123",
                ExpiryDate = "01/23",
                Currency = "GBP",
                Amount = 50.00
            };

            var response = acquiringBankStub.ProcessPaymentAsync(paymentRequest).Result;

            response.Status.Should().Be(TransactionStatus.Authorized);
            response.Id.GetType().Should().Be(typeof(Guid));
        }

        [Theory]
        [ClassData(typeof(PaymentRequestStringTestData))]
        public void ProcessInvalidPayment_WithInvalidPaymentRequestValues(PaymentRequest paymentRequest, TransactionStatus expectedStatus)
        {
            var expectedPaymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = expectedStatus
            };

            var response = acquiringBankStub.ProcessPaymentAsync(paymentRequest).Result;

            response.Status.Should().Be(expectedStatus);
            response.Id.GetType().Should().Be(typeof(Guid));
        }

        [Fact]
        public void ShouldProcessInvalidPayment_WithInvalidExpiryDateFormat()
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1111222233334444",
                CardHolderName = "A.Smith",
                CVV = "123",
                ExpiryDate = "01/20/23",
                Currency = "GBP",
                Amount = 50.00
            };

            var response = acquiringBankStub.ProcessPaymentAsync(paymentRequest).Result;

            response.Status.Should().Be(TransactionStatus.Declined);
            response.Id.GetType().Should().Be(typeof(Guid));
        }

        [Fact]
        public void ShouldProcessInvalidPayment_WithInvalidExpiryDateStringSyntax()
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1111222233334444",
                CardHolderName = "A.Smith",
                CVV = "123",
                ExpiryDate = "012023",
                Currency = "GBP",
                Amount = 50.00
            };

            var response = acquiringBankStub.ProcessPaymentAsync(paymentRequest).Result;

            response.Status.Should().Be(TransactionStatus.Declined);
            response.Id.GetType().Should().Be(typeof(Guid));
        }

        [Fact]
        public void ProcessInvalidPayment_WhenExpiryDateMonthIsNotInteger()
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1111222233334444",
                CardHolderName = "A.Smith",
                CVV = "123",
                ExpiryDate = "AB/23",
                Currency = "GBP",
                Amount = 50.00
            };

            var response = acquiringBankStub.ProcessPaymentAsync(paymentRequest).Result;

            response.Status.Should().Be(TransactionStatus.Declined);
            response.Id.GetType().Should().Be(typeof(Guid));
        }

        [Theory]
        [InlineData("13")]
        [InlineData("-1")]
        public void ProcessInvalidPayment_WhenExpiryDateMonthIsOutOfRange(string month)
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1111222233334444",
                CardHolderName = "A.Smith",
                CVV = "123",
                ExpiryDate = $"{month}/23",
                Currency = "GBP",
                Amount = 50.00
            };

            var response = acquiringBankStub.ProcessPaymentAsync(paymentRequest).Result;

            response.Status.Should().Be(TransactionStatus.Declined);
            response.Id.GetType().Should().Be(typeof(Guid));
        }

        [Fact]
        public void ProcessInvalidPayment_WhenExpiryDateYearIsNotInteger()
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1111222233334444",
                CardHolderName = "A.Smith",
                CVV = "123",
                ExpiryDate = "02/A3",
                Currency = "GBP",
                Amount = 50.00
            };

            var response = acquiringBankStub.ProcessPaymentAsync(paymentRequest).Result;

            response.Status.Should().Be(TransactionStatus.Declined);
            response.Id.GetType().Should().Be(typeof(Guid));
        }

        [Fact]
        public void ProcessInvalidPayment_WhenValidExpiryDateIsExpired()
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1111222233334444",
                CardHolderName = "A.Smith",
                CVV = "123",
                ExpiryDate = DateTime.UtcNow.AddMonths(-1).ToString("MM/yy"),
                Currency = "GBP",
                Amount = 50.00
            };

            var response = acquiringBankStub.ProcessPaymentAsync(paymentRequest).Result;

            response.Status.Should().Be(TransactionStatus.Declined);
            response.Id.GetType().Should().Be(typeof(Guid));
        }

        [Fact]
        public void ReturnNull_WhenPaymentRequestIsNull()
        {
            var response = acquiringBankStub.ProcessPaymentAsync(null).Result;

            response.Should().BeNull();
        }
    }
}
