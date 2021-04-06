using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pokedex.Application.Configuration;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Pokemon.PokeapiDtos;
using Pokedex.Infrastructure.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Pokedex.Infrastructure.WebRequests
{
    public class PokeapiSpeciesReader : IReader<string, PokemonSpecies>
    {
        private static readonly string _httpClientConfigurationName = "pokeapiClient";
        private readonly HttpClient _client;
        private static readonly JsonSerializer _serializer = new JsonSerializer()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public PokeapiSpeciesReader(HttpClient client, IOptions<HttpClientOptions> options)
        {
            _client = client;
            HttpClientConfiguration httpClientConfiguration = options?.Value?.ClientConfigurations
                ?.FirstOrDefault(cc => cc.Name == _httpClientConfigurationName);
            if (httpClientConfiguration == null)
                throw new InvalidOperationException($"Cannot create {nameof(FunTranslationClient)} without {nameof(HttpClientConfiguration)}");

            _client.BaseAddress = new Uri(httpClientConfiguration.BaseAddress);
            _client.Timeout = httpClientConfiguration.Timeout;
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<PokemonSpecies> Read(string name)
        {
            using (HttpResponseMessage response = await _client.GetAsync(name, HttpCompletionOption.ResponseHeadersRead))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync();
                return stream.DeserializeFromJson<PokemonSpecies>(_serializer);
            }
        }
    }
}
