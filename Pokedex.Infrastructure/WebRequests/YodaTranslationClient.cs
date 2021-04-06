using Microsoft.Extensions.Options;
using Pokedex.Application.Configuration;
using System.Net.Http;

namespace Pokedex.Infrastructure.WebRequests
{
    public class YodaTranslationClient : FunTranslationClient
    {
        public YodaTranslationClient(IHttpClientFactory httpClientFactory, IOptions<HttpClientOptions> options)
            : base(httpClientFactory, options)
        { }

        protected override string LastPartOfUrlPath { get { return "yoda.json"; } }
    }
}
