using System.Net.Http;

namespace Pokedex.Infrastructure.WebRequests
{
    public class ShakespeareTranslationClient: FunTranslationClient
    {
        public ShakespeareTranslationClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        { }

        protected override string LastPartOfUrlPath { get { return "shakespeare.json"; } }
    }
}
