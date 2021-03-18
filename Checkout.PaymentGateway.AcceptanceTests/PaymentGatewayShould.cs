namespace Checkout.PaymentGateway.AcceptanceTests
{
    using Checkout.PaymentGateway.Api;
    using Checkout.PaymentGateway.Models;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class PaymentGatewayShould : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected readonly HttpClient _client;
        private readonly string apiKey = "OnSzhN8q8ebQZys";
        public PaymentGatewayShould(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ProcessAuthorizedPaymentSuccessfully()
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

            HttpRequestMessage request = CreateProcessPaymentRequest(paymentRequest);

            var response = _client.SendAsync(request).Result;

            response.StatusCode.Should().Be(201);

            var paymentResponseJson = await response.Content.ReadAsStringAsync();
            var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(paymentResponseJson);

            paymentResponse.Status.Should().Be(Models.Enums.TransactionStatus.Authorized);
        }

        [Fact]
        public async Task ProcessDeclinedPaymentSuccessfully()
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1111222233334444",
                CardHolderName = "A.Smith",
                CVV = "123",
                ExpiryDate = "13/23",
                Currency = "GBP",
                Amount = 50.00,
            };

            HttpRequestMessage request = CreateProcessPaymentRequest(paymentRequest);

            var response = _client.SendAsync(request).Result;

            response.StatusCode.Should().Be(201);

            var paymentResponseJson = await response.Content.ReadAsStringAsync();
            var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(paymentResponseJson);

            paymentResponse.Status.Should().Be(Models.Enums.TransactionStatus.Declined);
        }

        [Fact]
        public async Task RetrieveHistoricPaymentSuccessfully()
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

            HttpRequestMessage request = CreateProcessPaymentRequest(paymentRequest);

            var response = _client.SendAsync(request).Result;

            response.StatusCode.Should().Be(201);

            var paymentResponseJson = await response.Content.ReadAsStringAsync();
            var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(paymentResponseJson);

            var paymentId = paymentResponse.Id;

            request = new HttpRequestMessage(HttpMethod.Get, "/api/payment/getpaymentdetail/" + paymentId.ToString());
            request.Headers.Add("ApiKey", apiKey);

            response = _client.SendAsync(request).Result;

            var paymentDetailJson = await response.Content.ReadAsStringAsync();
            var paymentDetail = JsonConvert.DeserializeObject<PaymentDetail>(paymentDetailJson);

            response.StatusCode.Should().Be(201);
            paymentDetail.Id.Should().Be(paymentResponse.Id);
            paymentDetail.Request.CardNumber.Should().Be(paymentRequest.MaskCardNumber());
        }

        [Fact]
        public async Task NotRetrieveInvalidHistoricalPayment()
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

            HttpRequestMessage request = CreateProcessPaymentRequest(paymentRequest);

            var response = _client.SendAsync(request).Result;

            response.StatusCode.Should().Be(201);

            var paymentResponseJson = await response.Content.ReadAsStringAsync();
            var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(paymentResponseJson);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/payment/getpaymentdetail/" + Guid.NewGuid().ToString());
            request.Headers.Add("ApiKey", apiKey);
            response = _client.SendAsync(request).Result;

            response.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ReturnUnauthorizedIfNoApiKeyPassed()
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

            HttpRequestMessage request = CreateUnauthorizedProcessPaymentRequest(paymentRequest);

            var response = _client.SendAsync(request).Result;

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task ReturnUnauthorizedIfIncorrectApiKeyPassed()
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

            HttpRequestMessage request = CreateIncorrectApiKeyProcessPaymentRequest(paymentRequest);

            var response = _client.SendAsync(request).Result;

            response.StatusCode.Should().Be(401);
        }

        private HttpRequestMessage CreateProcessPaymentRequest(PaymentRequest paymentRequest)
        {
            var serializedPaymentRequest = JsonConvert.SerializeObject(paymentRequest);

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/payment/makepayment")
            {
                Content = new StringContent(serializedPaymentRequest, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("ApiKey", apiKey);
            return request;
        }

        private HttpRequestMessage CreateUnauthorizedProcessPaymentRequest(PaymentRequest paymentRequest)
        {
            var serializedPaymentRequest = JsonConvert.SerializeObject(paymentRequest);

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/payment/makepayment")
            {
                Content = new StringContent(serializedPaymentRequest, Encoding.UTF8, "application/json")
            };
                       
            return request;
        }

        private HttpRequestMessage CreateIncorrectApiKeyProcessPaymentRequest(PaymentRequest paymentRequest)
        {
            var serializedPaymentRequest = JsonConvert.SerializeObject(paymentRequest);

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/payment/makepayment")
            {
                Content = new StringContent(serializedPaymentRequest, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("ApiKey", "1234");
            return request;
        }
    }
}
