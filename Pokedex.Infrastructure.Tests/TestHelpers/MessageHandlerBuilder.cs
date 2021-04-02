using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pokedex.Infrastructure.Tests.TestHelpers
{
    internal static class MessageHandlerBuilder
    {
        internal static HttpMessageHandler GetSuccessfulResponseMessageHandler(string responseContent)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            var mockSuccessfulResponseMessageHandler = new Mock<HttpMessageHandler>();
            mockSuccessfulResponseMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            return mockSuccessfulResponseMessageHandler.Object;
        }

        internal static HttpMessageHandler GetResponseMessageHandler(HttpStatusCode httpStatusCode)
        {
            var response = new HttpResponseMessage(httpStatusCode);

            var mockSuccessfulResponseMessageHandler = new Mock<HttpMessageHandler>();
            mockSuccessfulResponseMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            return mockSuccessfulResponseMessageHandler.Object;
        }
        internal static HttpMessageHandler GetExceptionThrowingMessageHandler()
        {
            var mockSuccessfulResponseMessageHandler = new Mock<HttpMessageHandler>();
            mockSuccessfulResponseMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("test exception"));

            return mockSuccessfulResponseMessageHandler.Object;
        }

    }
}
