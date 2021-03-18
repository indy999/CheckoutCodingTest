namespace Checkout.PaymentGateway.UnitTests.Controller
{
    using Checkout.PaymentGateway.Api.Controllers;
    using Checkout.PaymentGateway.Models;
    using Checkout.PaymentGateway.Models.Enums;
    using Checkout.PaymentGateway.Service.Interfaces;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class PaymentControllerShould
    {

        private Mock<IPaymentGateway> _paymentGatewayMock;
        private PaymentController paymentController;
        private Mock<ILogger<PaymentController>> _loggerMock;

        public PaymentControllerShould()
        {
            _paymentGatewayMock = new Mock<IPaymentGateway>();
            _loggerMock = new Mock<ILogger<PaymentController>>();
            paymentController = new PaymentController(_paymentGatewayMock.Object,_loggerMock.Object);
        }

        [Fact]
        public void ReturnHttpStatus201_OnSuccessfulAuthorizedPaymentRequest()
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

            _paymentGatewayMock.Setup(x => x.MakePaymentAsync(paymentRequest)).Returns(Task.FromResult(expectedPaymentResponse));

            var actionResult =  paymentController.MakePayment(paymentRequest).Result;
            var response = actionResult as ObjectResult;
            response.StatusCode.Should().Be(201);

            var paymentResponse = GetObjectResultContent<PaymentResponse>(response);
            paymentResponse.Status.Should().Be(TransactionStatus.Authorized);
            paymentResponse.Id.Should().Be(expectedPaymentResponse.Id);
        }

        [Fact]
        public void ReturnHttpStatus201_OnSuccessfulDeclinedPaymentRequest()
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
                Status = Models.Enums.TransactionStatus.Declined
            };

            _paymentGatewayMock.Setup(x => x.MakePaymentAsync(paymentRequest)).Returns(Task.FromResult(expectedPaymentResponse));

            var actionResult = paymentController.MakePayment(paymentRequest).Result;
            var response = actionResult as ObjectResult;
            response.StatusCode.Should().Be(201);

            var paymentResponse = GetObjectResultContent<PaymentResponse>(response);
            paymentResponse.Status.Should().Be(TransactionStatus.Declined);
            paymentResponse.Id.Should().Be(expectedPaymentResponse.Id);
        }

        [Fact]
        public void ReturnHttpStatus422_OnInvalidPaymentRequestDetails()
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "",
                CardHolderName = "A.Smith",
                CVV = "",
                ExpiryDate = "01/23",
                Currency = "GBP",
                Amount = 50.00
            };

            var actionResult = paymentController.MakePayment(paymentRequest).Result;
            var response = actionResult as StatusCodeResult;
            response.StatusCode.Should().Be(422);
        }

        [Fact]
        public void ReturnHttpStatus502_OnUnhandledExceptionDuringMakingPayment()
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

            _paymentGatewayMock.Setup(x => x.MakePaymentAsync(paymentRequest)).Throws(new Exception());

            var actionResult = paymentController.MakePayment(paymentRequest).Result;
            var response = actionResult as StatusCodeResult;
            response.StatusCode.Should().Be(502);
        }

        [Fact]
        public void ReturnHttpStatus201_OnSuccessfulPaymentDetailByIdRequest()
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

            var paymentDetail = new PaymentDetail(paymentRequest, expectedPaymentResponse);

            _paymentGatewayMock.Setup(x => x.GetPaymentDetailAsync(It.IsAny<Guid>())).Returns(Task.FromResult(paymentDetail));

            var actionResult = paymentController.GetPaymentDetail(expectedPaymentResponse.Id);
            var response = actionResult.Result as ObjectResult;
            response.StatusCode.Should().Be(201);
        }

        [Fact]
        public void ReturnHttpStatus404_OnUnsuccessfulPaymentDetailByIdRequest()
        {
            var paymentId = Guid.NewGuid();
            _paymentGatewayMock.Setup(x => x.GetPaymentDetailAsync(It.IsAny<Guid>())).Returns(Task.FromResult<PaymentDetail>(null));

            var actionResult = paymentController.GetPaymentDetail(paymentId);
            var response = actionResult.Result as StatusCodeResult;
            response.StatusCode.Should().Be(404);
        }

        private static T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }
    }
}
