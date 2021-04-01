using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pokedex.API.Mappers;
using Pokedex.API.Models;
using Pokedex.Domain;

namespace Pokedex.API.Tests.MappersTests
{
    [TestClass]
    public class PokemonMapperTests
    {
        private PokemonMapper _mapper;

        private static readonly Pokemon _legendaryPokemonWithRareHabitat = new Pokemon(
            "pokemon name",
            "pokemon description",
            "pokemon habitat",
            true
            );

        [TestInitialize]
        public void TestInit()
        {
            _mapper = new PokemonMapper();
        }

        [TestMethod]
        public void Map_LegendaryPokemonWithRareHabitat_IsNotNull()
        {
            PokemonDto dto = _mapper.Map(_legendaryPokemonWithRareHabitat);
            Assert.IsNotNull(dto);
        }

        [TestMethod]
        public void Map_LegendaryPokemonWithRareHabitat_HasCorrectName()
        {
            PokemonDto dto = _mapper.Map(_legendaryPokemonWithRareHabitat);
            Assert.AreEqual("pokemon name", dto.Name);
        }

        [TestMethod]
        public void Map_LegendaryPokemonWithRareHabitat_HasCorrectDescription()
        {
            PokemonDto dto = _mapper.Map(_legendaryPokemonWithRareHabitat);
            Assert.AreEqual("pokemon description", dto.Description);
        }

        [TestMethod]
        public void Map_LegendaryPokemonWithRareHabitat_HasCorrectHabitat()
        {
            PokemonDto dto = _mapper.Map(_legendaryPokemonWithRareHabitat);
            Assert.AreEqual("pokemon habitat", dto.Habitat);
        }

        [TestMethod]
        public void Map_LegendaryPokemonWithRareHabitat_IsLegendary()
        {
            PokemonDto dto = _mapper.Map(_legendaryPokemonWithRareHabitat);
            Assert.AreEqual(true, dto.IsLegendary);
        }
    }
}
