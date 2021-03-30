using Microsoft.AspNetCore.Mvc;
using Pokedex.API.Models;
using Pokedex.Application;
using Pokedex.Application.Pokemon;
using Pokedex.Domain;
using System.Threading.Tasks;

namespace Pokedex.API.Controllers
{
    [ApiController]
    public class PokemonController: ControllerBase
    {
        private readonly IPokemonService _pokemonService;
        private readonly IMapper<Pokemon, PokemonDto> _pokemonMapper;

        public PokemonController(IPokemonService pokemonService, IMapper<Pokemon,PokemonDto> pokemonMapper)
        {
            _pokemonService = pokemonService;
            _pokemonMapper = pokemonMapper;
        }

        [HttpGet("pokemon/{name}")]
        public async Task<ActionResult<PokemonDto>> Get([FromRoute] string name)
        {
            Pokemon pokemon = await _pokemonService.GetPokemon(name);

            if (pokemon == null)
                return NotFound();

            PokemonDto pokemonDto = _pokemonMapper.Map(pokemon);

            return Ok(pokemonDto);
        }
    }
}
