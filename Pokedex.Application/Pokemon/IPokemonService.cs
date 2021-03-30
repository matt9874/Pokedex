using System.Threading.Tasks;

namespace Pokedex.Application.Pokemon
{
    public interface IPokemonService
    {
        Task<Domain.Pokemon> GetPokemon(string name);
    }
}
