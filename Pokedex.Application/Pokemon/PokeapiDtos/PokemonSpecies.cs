using System.Collections.Generic;

namespace Pokedex.Application.Pokemon.PokeapiDtos
{
    public class PokemonSpecies
    {
        public string Name { get; set; }
        public List<FlavorTextEntry> FlavorTextEntries { get; set; }
        public Habitat Habitat { get; set; }
        public bool IsLegendary { get; set; }
    }
}
