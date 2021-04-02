using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.API.Controllers;
using Pokedex.API.Models;
using Pokedex.Application;
using Pokedex.Application.Pokemon;
using Pokedex.Application.Translation;
using Pokedex.Domain;
using System.Threading.Tasks;

namespace Pokedex.API.Tests.ControllersTests
{
    [TestClass]
    public class PokemonControllerTests
    {
        private Mock<IPokemonService> _mockPokemonService;
        private Mock<IMapper<Pokemon, PokemonDto>> _mockPokemonMapper;
        private Mock<ITranslationService> _mockTranslationService;
        private Mock<IMapper<Pokemon, TranslatedPokemonDto>> _mockTranslatedPokemonMapper;
        private PokemonController _controller;

        [TestInitialize]
        public void TestInit()
        {
            _mockPokemonService = new Mock<IPokemonService>();
            _mockPokemonMapper = new Mock<IMapper<Pokemon, PokemonDto>>();
            _mockTranslationService = new Mock<ITranslationService>();
            _mockTranslatedPokemonMapper = new Mock<IMapper<Pokemon, TranslatedPokemonDto>>();
            _controller = new PokemonController(
                _mockPokemonService.Object, 
                _mockPokemonMapper.Object, 
                _mockTranslationService.Object,
                _mockTranslatedPokemonMapper.Object);
        }

        private Pokemon GetNewPokemon()
        { 
            return new Pokemon(
                "name",
                new TranslatedText("description"),
                "habitat",
                true);
        }

        [TestMethod]
        public async Task Get_ServiceReturnsNull_ReturnsNotFound()
        {
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync((Pokemon)null);

            var getResult = await _controller.Get("name");

            Assert.IsInstanceOfType(getResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ServiceReturnNonNull_TranslationServiceNeverCalled()
        {
            Pokemon pokemon = GetNewPokemon();
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync(pokemon);

            var getResult = await _controller.Get("name");

            _mockTranslationService.Verify(ts => ts.Translate("description"), Times.Never);
        }

        [TestMethod]
        public async Task Get_ServiceAndMapperReturnNonNull_ReturnsOk()
        {
            Pokemon pokemon = GetNewPokemon();
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync(pokemon);
            _mockPokemonMapper.Setup(pm => pm.Map(pokemon)).Returns(new PokemonDto()
            {
                Name = "name",
                Description = "description",
                Habitat = "habitat",
                IsLegendary = true
            });

            var getResult = await _controller.Get("name");

            Assert.IsInstanceOfType(getResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Get_ServiceAndMapperReturnNonNull_ReturnsMappedDto()
        {
            Pokemon pokemon = GetNewPokemon();
            PokemonDto pokemonDto = new PokemonDto()
            {
                Name = "name",
                Description = "description",
                Habitat = "habitat",
                IsLegendary = true
            };
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync(pokemon);
            _mockPokemonMapper.Setup(pm => pm.Map(pokemon)).Returns(pokemonDto);

            var getResult = await _controller.Get("name");
            var okObjectResult = (OkObjectResult)getResult.Result;

            Assert.AreEqual(pokemonDto, okObjectResult.Value);
        }

        [TestMethod]
        public async Task GetTranslated_ServiceReturnsNull_ReturnsNotFound()
        {
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync((Pokemon)null);

            var getResult = await _controller.GetTranslated("name");

            Assert.IsInstanceOfType(getResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetTranslated_ServiceReturnNonNull_TranslationServiceCalledOnceOnDescription()
        {
            Pokemon pokemon = GetNewPokemon();
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync(pokemon);

            var getResult = await _controller.GetTranslated("name");

            _mockTranslationService.Verify(ts => ts.Translate(pokemon.Description.Text), Times.Once);
        }

        [TestMethod]
        public async Task GetTranslated_BothServicesReturnNonNull_MapperCalledOnPokemonWithTranslatedDescription()
        {
            Pokemon pokemon = GetNewPokemon();
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync(pokemon);
            var translation = new TranslatedText("translated description", TranslationType.Shakespeare);
            _mockTranslationService.Setup(ts => ts.Translate(pokemon.Description.Text))
                .ReturnsAsync(translation);

            var getResult = await _controller.GetTranslated("name");

            _mockTranslatedPokemonMapper.Verify(
                ts => ts.Map(It.Is<Pokemon>(
                    p=>p.Description == translation)), 
                Times.Once);
        }

        [TestMethod]
        public async Task GetTranslated_TranslationServiceReturnsDescription_ReturnsOk()
        {
            Pokemon pokemon = GetNewPokemon();
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync(pokemon);
            var translation = new TranslatedText("translated description", TranslationType.Shakespeare);
            _mockTranslationService.Setup(ts => ts.Translate("description"))
                .ReturnsAsync(translation);
            _mockPokemonMapper.Setup(pm => pm.Map(pokemon)).Returns(new PokemonDto()
            {
                Name = "name",
                Description = "translated description",
                Habitat = "habitat",
                IsLegendary = true
            });

            var getResult = await _controller.GetTranslated("name");

            Assert.IsInstanceOfType(getResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetTranslated_BothServicesAndMapperReturnNonNull_ReturnsMappedDto()
        {
            Pokemon pokemon = GetNewPokemon();
            var translatedPokemonDto = new TranslatedPokemonDto()
            {
                Name = "name",
                Description = "translated description",
                TranslationType = "Shakespeare",
                Habitat = "habitat",
                IsLegendary = true
            };
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync(pokemon);
            var translation = new TranslatedText("translated description", TranslationType.Shakespeare);
            _mockTranslationService.Setup(ts => ts.Translate("description"))
                .ReturnsAsync(translation);
            _mockTranslatedPokemonMapper.Setup(pm => pm.Map(pokemon)).Returns(translatedPokemonDto);

            var getResult = await _controller.GetTranslated("name");
            var okObjectResult = (OkObjectResult)getResult.Result;

            Assert.AreEqual(translatedPokemonDto, okObjectResult.Value);
        }
    }
}
