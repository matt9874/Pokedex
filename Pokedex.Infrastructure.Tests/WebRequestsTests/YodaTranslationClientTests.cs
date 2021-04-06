using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Application.Configuration;
using Pokedex.Application.Translation;
using Pokedex.Domain;
using Pokedex.Infrastructure.Tests.TestHelpers;
using Pokedex.Infrastructure.WebRequests;
using System;
using System.Collections.Generic;
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
        private Mock<IOptions<HttpClientOptions>> _httpClientConfiguration;
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
            _httpClientConfiguration = new Mock<IOptions<HttpClientOptions>>();
        }

        private void SetupConfiguration()
        {

            _httpClientConfiguration.Setup(cc => cc.Value).Returns(new HttpClientOptions()
            {
                ClientConfigurations = new List<HttpClientConfiguration>()
                {
                    new HttpClientConfiguration()
                    {
                        Name = "funTranslationClient",
                        BaseAddress = "http://theaddress.com/api/path/",
                        Timeout = TimeSpan.FromSeconds(30)
                    }
                }
            });
        }

        [TestMethod]
        public void Ctor_ConfigurationIsNull_ThrowsInvalidOperationException()
        {
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_exampleYodaJsonResponse);
            var client = new HttpClient(messageHandler);
            Assert.ThrowsException<InvalidOperationException>(() => new PokeapiSpeciesReader(client, _httpClientConfiguration.Object));
        }

        [TestMethod]
        public async Task Read_YodaExampleSuccessfulResponse_HasCorrectTranslation()
        {
            SetupConfiguration();
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_exampleYodaJsonResponse);
            var client = new HttpClient(messageHandler);
            _mockHttpClientFactory.Setup(cf => cf.CreateClient(It.IsAny<string>()))
                .Returns(client);
            var translationClient = new YodaTranslationClient(_mockHttpClientFactory.Object, _httpClientConfiguration.Object);

            var response = await translationClient.Read(_exampleYodaTranslationRequest);

            Assert.AreEqual("Mr,  you gave.Tim a hearty meal,Made him die,  but unfortunately what he ate.", response.Contents.Translated);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task Read_YodaExampleUnsuccessfulResponse_ThrowsHttpRequestException(HttpStatusCode httpStatusCode)
        {
            SetupConfiguration();
            var messageHandler = MessageHandlerBuilder.GetResponseMessageHandler(httpStatusCode);
            var client = new HttpClient(messageHandler);
            _mockHttpClientFactory.Setup(cf => cf.CreateClient(It.IsAny<string>()))
                .Returns(client);
            var translationClient = new YodaTranslationClient(_mockHttpClientFactory.Object, _httpClientConfiguration.Object);

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => translationClient.Read(_exampleYodaTranslationRequest));
        }

        [TestMethod]
        public async Task Read_YodaExampleMessageHandlerThrowsException_ThrowsHttpRequestException()
        {
            SetupConfiguration();
            var messageHandler = MessageHandlerBuilder.GetExceptionThrowingMessageHandler();
            var client = new HttpClient(messageHandler);
            _mockHttpClientFactory.Setup(cf => cf.CreateClient(It.IsAny<string>()))
                .Returns(client);
            var translationClient = new YodaTranslationClient(_mockHttpClientFactory.Object, _httpClientConfiguration.Object);

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => translationClient.Read(_exampleYodaTranslationRequest));
        }
    }
}
