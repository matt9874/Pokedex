using Pokedex.Domain;

namespace Pokedex.Application.Translation
{
    public interface ITranslationTypeDecider
    {
        TranslationType DecideTranslationType(Domain.Pokemon pokemon);
    }
}
