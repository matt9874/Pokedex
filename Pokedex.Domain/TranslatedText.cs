namespace Pokedex.Domain
{
    public class TranslatedText
    {
        public TranslatedText(string text)
        {
            Text = text;
            TranslationType = TranslationType.None;
        }

        public TranslatedText(string text, TranslationType translationType)
        {
            Text = text;
            TranslationType = translationType;
        }
        public string Text { get; }
        public TranslationType TranslationType { get; }
    }
}
