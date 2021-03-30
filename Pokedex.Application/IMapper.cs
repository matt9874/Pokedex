namespace Pokedex.Application
{
    public interface IMapper<TIn, TOut>
    {
        TOut Map(TIn data);
    }
}
