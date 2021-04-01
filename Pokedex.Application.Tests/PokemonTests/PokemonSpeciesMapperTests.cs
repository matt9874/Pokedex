using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pokedex.Application.Pokemon;
using Pokedex.Application.Pokemon.PokeapiDtos;
using System;
using System.Collections.Generic;

namespace Pokedex.Application.Tests.PokemonTests
{
    [TestClass]
    public class PokemonSpeciesMapperTests
    {
        private PokemonSpeciesMapper _mapper;

        [TestInitialize]
        public void TestInit()
        {
            _mapper = new PokemonSpeciesMapper();
        }

        [TestMethod]
        public void Map_Null_ThrowsArgumentNullException()
        {
            PokemonSpecies pokemonSpecies = null;

            Assert.ThrowsException<ArgumentNullException>(()=>_mapper.Map(pokemonSpecies));
        }

        [TestMethod]
        public void Map_NameIsNull_ThrowsArgumentNullException()
        {
            PokemonSpecies pokemonSpecies = new PokemonSpecies()
            { 
                FlavorTextEntries = new List<FlavorTextEntry> 
                {
                    new FlavorTextEntry 
                    {
                        FlavorText="description",
                        Language = new Language(){Name="en" }
                    }
                },
                Habitat = new Habitat() { Name = "Rare" },
                IsLegendary = true
            };

            Assert.ThrowsException<ArgumentNullException>(() => _mapper.Map(pokemonSpecies));
        }

        private PokemonSpecies _mewtwoPokemonSpecies = new PokemonSpecies()
        {
            Name = "mewtwo",
            FlavorTextEntries = new List<FlavorTextEntry>
            {
                new FlavorTextEntry
                {
                    FlavorText="Created by a scientist.",
                    Language = new Language(){Name="en" }
                }
            },
            Habitat = new Habitat() { Name = "rare" },
            IsLegendary = true
        };

        [TestMethod]
        public void Map_PropertiesAllValid_HasCorrectName()
        {
            Domain.Pokemon pokemon =  _mapper.Map(_mewtwoPokemonSpecies);

            Assert.AreEqual("mewtwo", pokemon.Name);
        }

        [TestMethod]
        public void Map_PropertiesAllValid_HasCorrectDescription()
        {
            Domain.Pokemon pokemon = _mapper.Map(_mewtwoPokemonSpecies);

            Assert.AreEqual("Created by a scientist.", pokemon.Description);
        }

        [TestMethod]
        public void Map_PropertiesAllValid_HasCorrectHabitat()
        {
            Domain.Pokemon pokemon = _mapper.Map(_mewtwoPokemonSpecies);

            Assert.AreEqual("rare", pokemon.Habitat);
        }

        [TestMethod]
        public void Map_IsLegendary_IsLegendary()
        {
            Domain.Pokemon pokemon = _mapper.Map(_mewtwoPokemonSpecies);

            Assert.AreEqual(true, pokemon.IsLegendary);
        }

        [TestMethod]
        public void Map_NullFlavorTextEntries_HasNullDescription()
        {
            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "mewtwo"
            };
            Domain.Pokemon pokemon = _mapper.Map(pokemonSpecies);

            Assert.IsNull(pokemon.Description);
        }

        [TestMethod]
        public void Map_EmptyFlavorTextEntries_HasNullDescription()
        {
            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "mewtwo",
                FlavorTextEntries = new List<FlavorTextEntry>()
            };
            Domain.Pokemon pokemon = _mapper.Map(pokemonSpecies);

            Assert.IsNull(pokemon.Description);
        }

        [TestMethod]
        public void Map_NoEnglishFlavorTextEntry_HasNullDescription()
        {
            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "mewtwo",
                FlavorTextEntries = new List<FlavorTextEntry>
                {
                    new FlavorTextEntry
                    {
                        FlavorText="not in English",
                        Language = new Language(){Name="fr" }
                    }
                }
            };
            Domain.Pokemon pokemon = _mapper.Map(pokemonSpecies);

            Assert.IsNull(pokemon.Description);
        }

        [TestMethod]
        public void Map_HyphenatedHabitat_HasCorrectHabitat()
        {
            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "mewtwo",
                Habitat = new Habitat() { Name = "waters-edge" }
            };
            Domain.Pokemon pokemon = _mapper.Map(pokemonSpecies);

            Assert.AreEqual("waters-edge", pokemon.Habitat);
        }

        [TestMethod]
        public void Map_IsNotLegendary_IsNotLegendary()
        {
            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "mewtwo",
                IsLegendary = false
            };
            Domain.Pokemon pokemon = _mapper.Map(pokemonSpecies);

            Assert.IsFalse(pokemon.IsLegendary);
        }
    }
}
