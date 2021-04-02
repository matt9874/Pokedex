using System.Threading.Tasks;

namespace Pokedex.Application.Translation
{
    public interface ITranslationService
    {
        Task<Domain.Pokemon> TranslateDescription(Domain.Pokemon text);
    }
}
