using Pokedex.Application.Interfaces;
using Pokedex.Application.Translation.Dtos;
using Pokedex.Domain;
using System.Threading.Tasks;

namespace Pokedex.Application.Translation
{
    public class FunTranslator : ITranslator
    {
        private readonly IReader<TranslationRequest, TranslationResult> _translationResultReader;

        public FunTranslator(IReader<TranslationRequest, TranslationResult> translationResultReader, TranslationType type)
        {
            _translationResultReader = translationResultReader;
            Type = type;
        }
        public TranslationType Type { get; }

        public async Task<string> Translate(string text)
        {
            var translationRequest = new TranslationRequest(Type, text);
            var translationResult = await _translationResultReader.Read(translationRequest);
            return translationResult?.Contents?.Translated;
        }
    }
}
