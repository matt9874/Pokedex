namespace Pokedex.API.Models
{
    public class TranslatedPokemonDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TranslationType { get; set; }
        public string Habitat { get; set; }
        public bool IsLegendary { get; set; }
    }
}
