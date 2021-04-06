using Pokedex.Domain;

namespace Pokedex.Application.Translation
{
    public class TranslationTypeDecider : ITranslationTypeDecider
    {
        private static readonly string _caveHabitatName = "cave";

        public TranslationType DecideTranslationType(Domain.Pokemon pokemon)
        {
            if (pokemon.Habitat == _caveHabitatName || pokemon.IsLegendary)
                return TranslationType.Yoda;

            return TranslationType.Shakespeare;
        }
    }
}
