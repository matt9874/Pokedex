using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Application.Translation;
using Pokedex.Domain;
using Pokedex.Infrastructure.Tests.TestHelpers;
using Pokedex.Infrastructure.WebRequests;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pokedex.Infrastructure.Tests.WebRequestsTests
{
    [TestClass]
    public class YodaTranslationClientTests
    {
        private static readonly TranslationRequest _exampleYodaTranslationRequest = new TranslationRequest(
            TranslationType.Yoda,
            "You gave Mr. Tim a hearty meal, but unfortunately what he ate made him die.");
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private const string _exampleYodaJsonResponse = @"
{
    ""success"": {
        ""total"": 1
    },
    ""contents"": {
        ""translated"": ""Mr,  you gave.Tim a hearty meal,Made him die,  but unfortunately what he ate."",
        ""text"": ""You gave Mr. Tim a hearty meal, but unfortunately what he ate made him die."",
        ""translation"": ""yoda""
    }
}";

        [TestInitialize]
        public void TestInit()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        }

        [TestMethod]
        public async Task Read_YodaExampleSuccessfulResponse_HasCorrectTranslation()
        {
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_exampleYodaJsonResponse);
            var client = new HttpClient(messageHandler);
            _mockHttpClientFactory.Setup(cf => cf.CreateClient(It.IsAny<string>()))
                .Returns(client);
            var translationClient = new YodaTranslationClient(_mockHttpClientFactory.Object);

            var response = await translationClient.Read(_exampleYodaTranslationRequest);

            Assert.AreEqual("Mr,  you gave.Tim a hearty meal,Made him die,  but unfortunately what he ate.", response.Contents.Translated);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task Read_YodaExampleUnsuccessfulResponse_ThrowsHttpRequestException(HttpStatusCode httpStatusCode)
        {
            var messageHandler = MessageHandlerBuilder.GetResponseMessageHandler(httpStatusCode);
            var client = new HttpClient(messageHandler);
            _mockHttpClientFactory.Setup(cf => cf.CreateClient(It.IsAny<string>()))
                .Returns(client);
            var translationClient = new YodaTranslationClient(_mockHttpClientFactory.Object);

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => translationClient.Read(_exampleYodaTranslationRequest));
        }

        [TestMethod]
        public async Task Read_YodaExampleMessageHandlerThrowsException_ThrowsHttpRequestException()
        {
            var messageHandler = MessageHandlerBuilder.GetExceptionThrowingMessageHandler();
            var client = new HttpClient(messageHandler);
            _mockHttpClientFactory.Setup(cf => cf.CreateClient(It.IsAny<string>()))
                .Returns(client);
            var translationClient = new YodaTranslationClient(_mockHttpClientFactory.Object);

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => translationClient.Read(_exampleYodaTranslationRequest));
        }
    }
}
