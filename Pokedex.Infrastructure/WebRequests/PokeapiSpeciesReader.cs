using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Pokemon.PokeapiDtos;
using Pokedex.Infrastructure.Extensions;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Pokedex.Infrastructure.WebRequests
{
    public class PokeapiSpeciesReader : IReader<string, PokemonSpecies>
    {
        private readonly HttpClient _client;
        private static readonly JsonSerializer _serializer = new JsonSerializer()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public PokeapiSpeciesReader(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon-species/");
            _client.Timeout = new TimeSpan(0, 0, 30);
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
