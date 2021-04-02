namespace Pokedex.Application.Translation
{
    public interface ITranslatorFactory
    {
        ITranslator CreateTranslator(Domain.Pokemon pokemon);
    }
}
