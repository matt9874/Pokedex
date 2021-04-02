using Pokedex.Domain;
using System.Threading.Tasks;

namespace Pokedex.Application.Translation
{
    public interface ITranslationService
    {
        Task<TranslatedText> Translate(string text);
    }
}
