using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.API.Controllers;
using Pokedex.API.Models;
using Pokedex.Application;
using Pokedex.Application.Pokemon;
using Pokedex.Domain;
using Pokedex.Domain.Enums;
using System.Threading.Tasks;

namespace Pokedex.API.Tests.ControllersTests
{
    [TestClass]
    public class PokemonControllerTests
    {
        private Mock<IPokemonService> _mockPokemonService;
        private Mock<IMapper<Pokemon, PokemonDto>> _mockPokemonMapper;
        private PokemonController _controller;

        [TestInitialize]
        public void TestInit()
        {
            _mockPokemonService = new Mock<IPokemonService>();
            _mockPokemonMapper = new Mock<IMapper<Pokemon, PokemonDto>>();
            _controller = new PokemonController(_mockPokemonService.Object, _mockPokemonMapper.Object);
        }

        [TestMethod]
        public async Task Get_ServiceReturnsNull_ReturnsNotFound()
        {
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync((Pokemon)null);

            var getResult = await _controller.Get("name");

            Assert.IsInstanceOfType(getResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ServiceAndMapperReturnNonNull_ReturnsOk()
        {
            Pokemon pokemon = new Pokemon(
                "name",
                "description",
                Habitat.Cave,
                true);

            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync(pokemon);
            _mockPokemonMapper.Setup(pm => pm.Map(pokemon)).Returns(new PokemonDto()
            {
                Name = "name",
                Description = "description",
                Habitat = "Cave",
                IsLegendary = true
            });

            var getResult = await _controller.Get("name");

            Assert.IsInstanceOfType(getResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Get_ServiceAndMapperReturnNonNull_ReturnsMappedDto()
        {
            Pokemon pokemon = new Pokemon(
                "name",
                "description",
                Habitat.Cave,
                true);
            PokemonDto pokemonDto = new PokemonDto()
            {
                Name = "name",
                Description = "description",
                Habitat = "Cave",
                IsLegendary = true
            };
            _mockPokemonService.Setup(ps => ps.GetPokemon(It.IsAny<string>())).ReturnsAsync(pokemon);
            _mockPokemonMapper.Setup(pm => pm.Map(pokemon)).Returns(pokemonDto);

            var getResult = await _controller.Get("name");
            var okObjectResult = (OkObjectResult)getResult.Result;

            Assert.AreEqual(pokemonDto, okObjectResult.Value);
        }
    }
}
