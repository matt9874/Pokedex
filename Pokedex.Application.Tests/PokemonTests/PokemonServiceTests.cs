using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pokedex.Application.Pokemon;
using Pokedex.Application.Pokemon.PokeapiDtos;
using Moq;
using Pokedex.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace Pokedex.Application.Tests.PokemonTests
{
    [TestClass]
    public class PokemonServiceTests
    {
        private Mock<IReader<string, PokemonSpecies>> _mockPokemonReader;
        private Mock<IMapper<PokemonSpecies, Domain.Pokemon>> _mockPokemonMapper;
        private PokemonService _pokemonService;

        [TestInitialize]
        public void TestInit()
        {
            _mockPokemonReader = new Mock<IReader<string, PokemonSpecies>>();
            _mockPokemonMapper = new Mock<IMapper<PokemonSpecies, Domain.Pokemon>>();
            _pokemonService = new PokemonService(_mockPokemonReader.Object, _mockPokemonMapper.Object);
        }

        [TestMethod]
        public async Task GetPokemon_Null_ThrowsArgumentNullException()
        {
            string name = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _pokemonService.GetPokemon(name));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("  ")]
        public async Task GetPokemon_Whitespace_ThrowsArgumentOutOfRangeException(string name)
        {
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _pokemonService.GetPokemon(name));
        }

        [TestMethod]
        [DataRow("name")]
        public async Task GetPokemon_ValidName_ReaderIsCalled(string name)
        {
            Domain.Pokemon pokemon = await _pokemonService.GetPokemon(name);
            _mockPokemonReader.Verify(pr=>pr.Read(name));
        }

        [TestMethod]
        [DataRow("name")]
        public async Task GetPokemon_ValidName_ResultOfMapperIsReturned(string name)
        {
            var mappedPokemon = new Domain.Pokemon("name", "description", "habitat", true);
            _mockPokemonMapper.Setup(m => m.Map(It.IsAny<PokemonSpecies>()))
                .Returns(mappedPokemon);

            Domain.Pokemon returnedPokemon = await _pokemonService.GetPokemon(name);

            Assert.AreEqual(mappedPokemon, returnedPokemon);
        }
    }
}
