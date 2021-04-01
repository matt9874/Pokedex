using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Pokedex.Infrastructure.WebRequests;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pokedex.Infrastructure.Tests.WebRequestsTests
{
    [TestClass]
    public class PokeapiSpeciesReaderTests
    {
        private const string _simplifiedMewtwoResponseContent = @"
{
    ""flavor_text_entries"": [
        {
            ""flavor_text"": ""It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments."",
            ""language"": {
                ""name"": ""en"",
                ""url"": ""https://pokeapi.co/api/v2/language/9/""
            },
            ""version"": {
                ""name"": ""red"",
                ""url"": ""https://pokeapi.co/api/v2/version/1/""
            }
        },{
            ""flavor_text"": ""身体上的肌肉因精神力量\n而增强。它的握力为１吨。\n只要２秒就可以跑完100米！"",
            ""language"": {
                ""name"": ""zh-Hans"",
                ""url"": ""https://pokeapi.co/api/v2/language/12/""
            },
            ""version"": {
                ""name"": ""lets-go-eevee"",
                ""url"": ""https://pokeapi.co/api/v2/version/32/""
            }
        }
    ],
    ""habitat"": {
        ""name"": ""rare"",
        ""url"": ""https://pokeapi.co/api/v2/pokemon-habitat/5/""
    },
    ""is_legendary"": true,
    ""name"": ""mewtwo""
}";

        private HttpMessageHandler GetSuccessfulResponseMessageHandler(string responseContent)
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

        private HttpMessageHandler GetResponseMessageHandler(HttpStatusCode httpStatusCode)
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
        private HttpMessageHandler GetExceptionThrowingMessageHandler()
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

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_IsNotNull()
        {
            var messageHandler = GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client);

            var pokemonSpecies = await reader.Read("name");

            Assert.IsNotNull(pokemonSpecies);
        }

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_CorrectName()
        {
            var messageHandler = GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client);

            var pokemonSpecies = await reader.Read("name");

            Assert.AreEqual("mewtwo", pokemonSpecies.Name);
        }

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_FlavorTextEntriesHasCountTwo()
        {
            var messageHandler = GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client);

            var pokemonSpecies = await reader.Read("name");

            Assert.AreEqual(2, pokemonSpecies.FlavorTextEntries.Count);
        }

        [TestMethod]
        [DataRow("It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.", "en")]
        [DataRow("身体上的肌肉因精神力量\n而增强。它的握力为１吨。\n只要２秒就可以跑完100米！", "zh-Hans")]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_HasFlavorTextEntry(string flavorText, string languageName)
        {
            var messageHandler = GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client);

            var pokemonSpecies = await reader.Read("name");

            Assert.IsTrue(pokemonSpecies.FlavorTextEntries.Any(fte => fte.FlavorText == flavorText && fte.Language.Name == languageName));
        }

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_HabitatIsRare()
        {
            var messageHandler = GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client);

            var pokemonSpecies = await reader.Read("name");

            Assert.AreEqual("rare", pokemonSpecies.Habitat.Name);
        }

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_IsLegendary()
        {
            var messageHandler = GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client);

            var pokemonSpecies = await reader.Read("name");

            Assert.IsTrue(pokemonSpecies.IsLegendary);
        }

        [TestMethod]
        public async Task Read_ClientReturnsNotFound_ReturnsNull()
        {
            var client = new HttpClient(GetResponseMessageHandler(HttpStatusCode.NotFound));
            var reader = new PokeapiSpeciesReader(client);

            var pokemonSpecies = await reader.Read("name");

            Assert.IsNull(pokemonSpecies);
        }

        [TestMethod]
        public async Task Read_ClientReturnsInternalServerError_ThrowsHttpRequestException()
        {
            var client = new HttpClient(GetResponseMessageHandler(HttpStatusCode.InternalServerError));
            var reader = new PokeapiSpeciesReader(client);

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => reader.Read("name"));
        }

        [TestMethod]
        public async Task Read_ClientThrowsException_ThrowsHttpRequestException()
        {
            var client = new HttpClient(GetExceptionThrowingMessageHandler());
            var reader = new PokeapiSpeciesReader(client);

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => reader.Read("name"));
        }
    }
}
