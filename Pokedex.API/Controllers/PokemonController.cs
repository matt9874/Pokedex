using Microsoft.AspNetCore.Mvc;
using Pokedex.API.Models;
using Pokedex.Application;
using Pokedex.Application.Pokemon;
using Pokedex.Application.Translation;
using Pokedex.Domain;
using System.Threading.Tasks;

namespace Pokedex.API.Controllers
{
    [ApiController]
    public class PokemonController: ControllerBase
    {
        private readonly IPokemonService _pokemonService;
        private readonly IMapper<Pokemon, PokemonDto> _pokemonMapper;
        private readonly ITranslationService _translationService;
        private readonly IMapper<Pokemon, TranslatedPokemonDto> _translatedPokemonMapper;

        public PokemonController(IPokemonService pokemonService, IMapper<Pokemon,PokemonDto> pokemonMapper,
            ITranslationService translationService, IMapper<Pokemon, TranslatedPokemonDto> translatedPokemonMapper)
        {
            _pokemonService = pokemonService;
            _pokemonMapper = pokemonMapper;
            _translationService = translationService;
            _translatedPokemonMapper = translatedPokemonMapper;
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

        [HttpGet("pokemon/translated/{name}")]
        public async Task<ActionResult<PokemonDto>> GetTranslated([FromRoute] string name)
        {
            Pokemon pokemon = await _pokemonService.GetPokemon(name);

            if (pokemon == null)
                return NotFound();

            TranslatedText translatedDescription = await _translationService.Translate(pokemon.Description.Text);
            pokemon = new Pokemon(pokemon.Name, translatedDescription, pokemon.Habitat, pokemon.IsLegendary);

            TranslatedPokemonDto pokemonDto = _translatedPokemonMapper.Map(pokemon);

            return Ok(pokemonDto);
        }
    }
}
