using Pokedex.Application.Interfaces;
using Pokedex.Application.Translation.Dtos;
using Pokedex.Domain;
using System;
using System.Threading.Tasks;

namespace Pokedex.Application.Translation
{
    public class TranslationService : ITranslationService
    {
        private readonly ITranslationTypeDecider _translationTypeDecider;
        private readonly Func<TranslationType, IReader<TranslationRequest, TranslationResult>> _translatorFactory;

        public TranslationService(ITranslationTypeDecider translationTypeDecider, 
            Func<TranslationType, IReader<TranslationRequest, TranslationResult>> translatorFactory)
        {
            _translationTypeDecider = translationTypeDecider;
            _translatorFactory = translatorFactory;
        }
        public async Task<Domain.Pokemon> TranslateDescription(Domain.Pokemon pokemon)
        {
            if (pokemon == null || pokemon.Description == null || string.IsNullOrWhiteSpace(pokemon.Description.Text))
                return pokemon;

            TranslationType translationType = _translationTypeDecider.DecideTranslationType(pokemon);
            IReader<TranslationRequest, TranslationResult> translator = _translatorFactory?.Invoke(translationType);
            TranslationRequest translationRequest = new TranslationRequest(translationType, pokemon.Description.Text);

            try
            {
                TranslationResult translatedResult = await translator.Read(translationRequest);
                TranslatedText translatedText = new TranslatedText(translatedResult.Contents.Translated, translationType);
                return new Domain.Pokemon(pokemon.Name, translatedText, pokemon.Habitat, pokemon.IsLegendary);
            }
            catch
            {
                return pokemon;
            }
        }
    }
}
