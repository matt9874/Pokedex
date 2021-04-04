using Pokedex.Application.Interfaces;
using Pokedex.Application.Translation.Dtos;
using Pokedex.Domain;

namespace Pokedex.Application.Translation
{
    public class TranslatorFactory : ITranslatorFactory
    {
        private static readonly string _caveHabitatName = "cave";
        private readonly IReader<TranslationRequest, TranslationResult> _translationResultReader;

        public TranslatorFactory(IReader<TranslationRequest, TranslationResult> translationResultReader)
        {
            _translationResultReader = translationResultReader;
        }
        public ITranslator CreateTranslator(Domain.Pokemon pokemon)
        {
            var translationType = TranslationType.Shakespeare;

            if (pokemon.Habitat == _caveHabitatName || pokemon.IsLegendary)
                translationType = TranslationType.Yoda;

            return new FunTranslator(_translationResultReader, translationType);
        }
    }
}
