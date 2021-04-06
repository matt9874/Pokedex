using System.Net.Http;

namespace Pokedex.Infrastructure.WebRequests
{
    public class YodaTranslationClient : FunTranslationClient
    {
        public YodaTranslationClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        { }

        protected override string LastPartOfUrlPath { get { return "yoda.json"; } }
    }
}
