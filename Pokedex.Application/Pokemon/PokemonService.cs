using Pokedex.Application.Interfaces;
using Pokedex.Application.Pokemon.PokeapiDtos;
using System;
using System.Threading.Tasks;

namespace Pokedex.Application.Pokemon
{
    public class PokemonService : IPokemonService
    {
        private readonly IReader<string, PokemonSpecies> _pokemonReader;
        private readonly IMapper<PokemonSpecies, Domain.Pokemon> _pokemonMapper;

        public PokemonService(IReader<string, PokemonSpecies> pokemonReader, 
            IMapper<PokemonSpecies, Domain.Pokemon> pokemonMapper)
        {
            _pokemonReader = pokemonReader;
            _pokemonMapper = pokemonMapper;
        }
        public async Task<Domain.Pokemon> GetPokemon(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException("name", "Pokemon name cannot be whitespace");

            PokemonSpecies pokemonSpecies = await _pokemonReader.Read(name);

            return _pokemonMapper.Map(pokemonSpecies);
        }
    }
}
