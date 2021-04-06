using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Application.Configuration;
using Pokedex.Infrastructure.Tests.TestHelpers;
using Pokedex.Infrastructure.WebRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        private Mock<IOptions<HttpClientOptions>> _httpClientConfiguration;

        [TestInitialize]
        public void TestInit()
        {
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
                        Name = "pokeapiClient",
                        BaseAddress = "http://theaddress.com/api/path/",
                        Timeout = TimeSpan.FromSeconds(30)
                    }
                }
            });
        }

        [TestMethod]
        public void Ctor_ConfigurationIsNull_ThrowsInvalidOperationException()
        {
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            Assert.ThrowsException<InvalidOperationException>(() => new PokeapiSpeciesReader(client, _httpClientConfiguration.Object));
        }

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_IsNotNull()
        {
            SetupConfiguration();
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client, _httpClientConfiguration.Object);

            var pokemonSpecies = await reader.Read("name");

            Assert.IsNotNull(pokemonSpecies);
        }

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_CorrectName()
        {
            SetupConfiguration();
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client, _httpClientConfiguration.Object);

            var pokemonSpecies = await reader.Read("name");

            Assert.AreEqual("mewtwo", pokemonSpecies.Name);
        }

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_FlavorTextEntriesHasCountTwo()
        {
            SetupConfiguration();
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client, _httpClientConfiguration.Object);

            var pokemonSpecies = await reader.Read("name");

            Assert.AreEqual(2, pokemonSpecies.FlavorTextEntries.Count);
        }

        [TestMethod]
        [DataRow("It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.", "en")]
        [DataRow("身体上的肌肉因精神力量\n而增强。它的握力为１吨。\n只要２秒就可以跑完100米！", "zh-Hans")]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_HasFlavorTextEntry(string flavorText, string languageName)
        {
            SetupConfiguration();
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client, _httpClientConfiguration.Object);

            var pokemonSpecies = await reader.Read("name");

            Assert.IsTrue(pokemonSpecies.FlavorTextEntries.Any(fte => fte.FlavorText == flavorText && fte.Language.Name == languageName));
        }

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_HabitatIsRare()
        {
            SetupConfiguration();
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client, _httpClientConfiguration.Object);

            var pokemonSpecies = await reader.Read("name");

            Assert.AreEqual("rare", pokemonSpecies.Habitat.Name);
        }

        [TestMethod]
        public async Task Read_ClientReturnsSimplifiedMewtwoJson_IsLegendary()
        {
            SetupConfiguration();
            var messageHandler = MessageHandlerBuilder.GetSuccessfulResponseMessageHandler(_simplifiedMewtwoResponseContent);
            var client = new HttpClient(messageHandler);
            var reader = new PokeapiSpeciesReader(client, _httpClientConfiguration.Object);

            var pokemonSpecies = await reader.Read("name");

            Assert.IsTrue(pokemonSpecies.IsLegendary);
        }

        [TestMethod]
        public async Task Read_ClientReturnsNotFound_ReturnsNull()
        {
            SetupConfiguration();
            var client = new HttpClient(MessageHandlerBuilder.GetResponseMessageHandler(HttpStatusCode.NotFound));
            var reader = new PokeapiSpeciesReader(client, _httpClientConfiguration.Object);

            var pokemonSpecies = await reader.Read("name");

            Assert.IsNull(pokemonSpecies);
        }

        [TestMethod]
        public async Task Read_ClientReturnsInternalServerError_ThrowsHttpRequestException()
        {
            SetupConfiguration();
            var client = new HttpClient(MessageHandlerBuilder.GetResponseMessageHandler(HttpStatusCode.InternalServerError));
            var reader = new PokeapiSpeciesReader(client, _httpClientConfiguration.Object);

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => reader.Read("name"));
        }

        [TestMethod]
        public async Task Read_ClientThrowsException_ThrowsHttpRequestException()
        {
            SetupConfiguration();
            var client = new HttpClient(MessageHandlerBuilder.GetExceptionThrowingMessageHandler());
            var reader = new PokeapiSpeciesReader(client, _httpClientConfiguration.Object);

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => reader.Read("name"));
        }
    }
}
