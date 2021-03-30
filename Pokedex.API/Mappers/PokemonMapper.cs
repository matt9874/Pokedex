using Pokedex.Domain;
using Pokedex.Application;
using Pokedex.API.Models;

namespace Pokedex.API.Mappers
{
    public class PokemonMapper : IMapper<Pokemon, PokemonDto>
    {
        public PokemonDto Map(Pokemon pokemon)
        {
            return new PokemonDto()
            {
                Name = pokemon.Name,
                Description = pokemon.Description,
                Habitat = pokemon.Habitat.ToString(),
                IsLegendary = pokemon.IsLegendary
            };
        }
    }
}
