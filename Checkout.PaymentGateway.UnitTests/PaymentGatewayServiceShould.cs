namespace Checkout.PaymentGateway.UnitTests
{
    using Checkout.PaymentGateway.Models;
    using Checkout.PaymentGateway.Models.Enums;
    using Checkout.PaymentGateway.Service;
    using Checkout.PaymentGateway.Service.Interfaces;
    using FluentAssertions;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class PaymentGatewayServiceShould
    {
        private Mock<IBank> _acquiringBankMock;
        private Mock<IRepository> _repositoryMock;

        private PaymentGatewayService paymentGatewayService;

        public PaymentGatewayServiceShould()
        {
            _acquiringBankMock = new Mock<IBank>();
            _repositoryMock = new Mock<IRepository>();
            paymentGatewayService = new PaymentGatewayService(_acquiringBankMock.Object, _repositoryMock.Object);
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

            var expectedPaymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = Models.Enums.TransactionStatus.Authorized
            };

            _acquiringBankMock.Setup(x => x.ProcessPaymentAsync(paymentRequest))
                .Returns(Task.FromResult(expectedPaymentResponse));

            var result = paymentGatewayService.MakePaymentAsync(paymentRequest).Result;

            result.Status.Should().Be(TransactionStatus.Authorized);
            result.Id.Should().Be(expectedPaymentResponse.Id);
            _repositoryMock.Verify(x => x.SavePaymentDetailAsync(It.IsAny<PaymentDetail>()), Times.Once);
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

            _acquiringBankMock.Setup(x => x.ProcessPaymentAsync(paymentRequest))
                .Returns(Task.FromResult(expectedPaymentResponse));

            var result = paymentGatewayService.MakePaymentAsync(paymentRequest).Result;

            result.Status.Should().Be(expectedStatus);
            result.Id.Should().Be(expectedPaymentResponse.Id);
            _repositoryMock.Verify(x => x.SavePaymentDetailAsync(It.IsAny<PaymentDetail>()), Times.Once);
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

            var expectedPaymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = Models.Enums.TransactionStatus.Declined
            };

            _acquiringBankMock.Setup(x => x.ProcessPaymentAsync(paymentRequest))
                .Returns(Task.FromResult(expectedPaymentResponse));

            var result = paymentGatewayService.MakePaymentAsync(paymentRequest).Result;

            result.Status.Should().Be(TransactionStatus.Declined);
            result.Id.Should().Be(expectedPaymentResponse.Id);
            _repositoryMock.Verify(x => x.SavePaymentDetailAsync(It.IsAny<PaymentDetail>()), Times.Once);
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

            var expectedPaymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = Models.Enums.TransactionStatus.Declined
            };

            _acquiringBankMock.Setup(x => x.ProcessPaymentAsync(paymentRequest))
                .Returns(Task.FromResult(expectedPaymentResponse));

            var result = paymentGatewayService.MakePaymentAsync(paymentRequest).Result;

            result.Status.Should().Be(TransactionStatus.Declined);
            result.Id.Should().Be(expectedPaymentResponse.Id);
            _repositoryMock.Verify(x => x.SavePaymentDetailAsync(It.IsAny<PaymentDetail>()), Times.Once);
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

            var expectedPaymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = Models.Enums.TransactionStatus.Declined
            };

            _acquiringBankMock.Setup(x => x.ProcessPaymentAsync(paymentRequest))
                .Returns(Task.FromResult(expectedPaymentResponse));

            var result = paymentGatewayService.MakePaymentAsync(paymentRequest).Result;

            result.Status.Should().Be(TransactionStatus.Declined);
            result.Id.Should().Be(expectedPaymentResponse.Id);
            _repositoryMock.Verify(x => x.SavePaymentDetailAsync(It.IsAny<PaymentDetail>()), Times.Once);
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

            var expectedPaymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = Models.Enums.TransactionStatus.Declined
            };

            _acquiringBankMock.Setup(x => x.ProcessPaymentAsync(paymentRequest))
                .Returns(Task.FromResult(expectedPaymentResponse));

            var result = paymentGatewayService.MakePaymentAsync(paymentRequest).Result;

            result.Status.Should().Be(TransactionStatus.Declined);
            result.Id.Should().Be(expectedPaymentResponse.Id);
            _repositoryMock.Verify(x => x.SavePaymentDetailAsync(It.IsAny<PaymentDetail>()), Times.Once);
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

            var expectedPaymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = Models.Enums.TransactionStatus.Declined
            };

            _acquiringBankMock.Setup(x => x.ProcessPaymentAsync(paymentRequest))
                .Returns(Task.FromResult(expectedPaymentResponse));

            var result = paymentGatewayService.MakePaymentAsync(paymentRequest).Result;

            result.Status.Should().Be(TransactionStatus.Declined);
            result.Id.Should().Be(expectedPaymentResponse.Id);
            _repositoryMock.Verify(x => x.SavePaymentDetailAsync(It.IsAny<PaymentDetail>()), Times.Once);
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

            var expectedPaymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = Models.Enums.TransactionStatus.Declined
            };

            _acquiringBankMock.Setup(x => x.ProcessPaymentAsync(paymentRequest))
                .Returns(Task.FromResult(expectedPaymentResponse));

            var response = paymentGatewayService.MakePaymentAsync(paymentRequest).Result;

            response.Status.Should().Be(TransactionStatus.Declined);
            response.Id.Should().Be(expectedPaymentResponse.Id);
            _repositoryMock.Verify(x => x.SavePaymentDetailAsync(It.IsAny<PaymentDetail>()), Times.Once);
        }

        [Fact]
        public void ReturnNull_WhenPaymentRequestIsNull()
        {
            _acquiringBankMock.Setup(x => x.ProcessPaymentAsync(null)).Returns(Task.FromResult<PaymentResponse>(null));

            var response = paymentGatewayService.MakePaymentAsync(null).Result;

            response.Should().BeNull();
        }

        [Fact]
        public void ReturnPaymentDetail()
        {
            var paymentId = Guid.NewGuid();

            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1111222233334444",
                CardHolderName = "A.Smith",
                CVV = "123",
                ExpiryDate = "01/23",
                Currency = "GBP",
                Amount = 50.00
            };

            var expectedPaymentResponse = new PaymentResponse
            {
                Id = paymentId,
                Status = Models.Enums.TransactionStatus.Authorized
            };

            var expectedPaymentDetail = new PaymentDetail(paymentRequest, expectedPaymentResponse);

            _repositoryMock.Setup(x => x.GetPaymentDetailAsync(paymentId))
                .Returns(Task.FromResult(expectedPaymentDetail));

            var paymentDetail = paymentGatewayService.GetPaymentDetailAsync(paymentId).Result;

            paymentDetail.Id = expectedPaymentResponse.Id;
            paymentDetail.Response.Status.Should().Be(expectedPaymentResponse.Status);
            paymentDetail.Request.CardHolderName.Should().Be(paymentRequest.CardHolderName);
            paymentDetail.Request.CardNumber.Should().Be(paymentRequest.MaskCardNumber());
        }

        [Fact]
        public void ReturnNullIfPaymentDetailNotFound()
        {
            var paymentId = Guid.NewGuid();

            _repositoryMock.Setup(x => x.GetPaymentDetailAsync(paymentId))
                .Returns(Task.FromResult<PaymentDetail>(null));

            var paymentDetail = paymentGatewayService.GetPaymentDetailAsync(paymentId).Result;

            paymentDetail.Should().BeNull();
        }
    }
}

