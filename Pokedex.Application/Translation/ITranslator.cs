using Pokedex.Domain;
using System.Threading.Tasks;

namespace Pokedex.Application.Translation
{
    public interface ITranslator
    {
        TranslationType Type { get; }
        Task<string> Translate(string text);
    }
}
