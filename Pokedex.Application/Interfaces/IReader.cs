using System.Threading.Tasks;

namespace Pokedex.Application.Interfaces
{
    public interface IReader<TId, TEntity>
    {
        Task<TEntity> Read(TId id);
    }
}
