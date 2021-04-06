using Microsoft.Extensions.Options;
using Pokedex.Application.Configuration;
using System.Net.Http;

namespace Pokedex.Infrastructure.WebRequests
{
    public class ShakespeareTranslationClient: FunTranslationClient
    {
        public ShakespeareTranslationClient(IHttpClientFactory httpClientFactory, IOptions<HttpClientOptions> options) 
            : base(httpClientFactory, options)
        { }

        protected override string LastPartOfUrlPath { get { return "shakespeare.json"; } }
    }
}
