using Pokedex.Domain;
using System.Threading.Tasks;

namespace Pokedex.Application.Translation
{
    public class TranslationService : ITranslationService
    {
        private readonly ITranslatorFactory _translatorFactory;

        public TranslationService(ITranslatorFactory translatorFactory)
        {
            _translatorFactory = translatorFactory;
        }
        public async Task<Domain.Pokemon> TranslateDescription(Domain.Pokemon pokemon)
        {
            if (pokemon == null || pokemon.Description == null || string.IsNullOrWhiteSpace(pokemon.Description.Text))
                return pokemon;

            ITranslator translator = _translatorFactory.CreateTranslator(pokemon);
            TranslatedText translatedDescriptionWithType;

            try
            {
                string translatedDescription = await translator.Translate(pokemon.Description.Text);
                translatedDescriptionWithType = new TranslatedText(translatedDescription, translator.Type);
                return new Domain.Pokemon(pokemon.Name, translatedDescriptionWithType, pokemon.Habitat, pokemon.IsLegendary);
            }
            catch
            {
                return pokemon;
            }
        }
    }
}
