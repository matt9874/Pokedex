using Pokedex.Application.Pokemon.PokeapiDtos;
using System;
using System.Linq;

namespace Pokedex.Application.Pokemon
{
    public class PokemonSpeciesMapper : IMapper<PokemonSpecies, Domain.Pokemon>
    {
        public Domain.Pokemon Map(PokemonSpecies pokemonSpecies)
        {
            if (pokemonSpecies == null)
                throw new ArgumentNullException("pokemonSpecies");

            FlavorTextEntry firstEnglishFlavorTextEntry = pokemonSpecies.FlavorTextEntries?.FirstOrDefault(
                fte => fte?.Language?.Name == "en");

            return new Domain.Pokemon(
                pokemonSpecies.Name,
                firstEnglishFlavorTextEntry?.FlavorText,
                pokemonSpecies.Habitat?.Name,
                pokemonSpecies.IsLegendary
                ); ;
        }
    }
}
