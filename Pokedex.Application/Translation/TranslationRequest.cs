using Pokedex.Domain;
using System;

namespace Pokedex.Application.Translation
{
    public class TranslationRequest
    {
        public TranslationRequest(TranslationType type, string text)
        {
            if (type == TranslationType.None)
                throw new ArgumentOutOfRangeException("type", "Translation type cannot be None");
            Type = type;
            Text = text;
        }
        public TranslationType Type { get; }
        public string Text { get; }
    }
}
