using Pokedex.API.Models;
using Pokedex.Application;
using Pokedex.Domain;

namespace Pokedex.API.Mappers
{
    public class TranslatedPokemonMapper : IMapper<Pokemon, TranslatedPokemonDto>
    {
        public TranslatedPokemonDto Map(Pokemon pokemon)
        {
            TranslationType translationType = pokemon.Description == null ? TranslationType.None
                : pokemon.Description.TranslationType;
            return new TranslatedPokemonDto()
            {
                Name = pokemon.Name,
                Description = pokemon.Description?.Text,
                TranslationType = translationType.ToString(),
                Habitat = pokemon.Habitat.ToString(),
                IsLegendary = pokemon.IsLegendary
            };
        }
    }
}
