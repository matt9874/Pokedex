using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Pokedex.Application.Translation.Dtos;
using Pokedex.Domain;
using Pokedex.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Translation;


namespace Pokedex.Infrastructure.WebRequests
{
    public class FunTranslationClient: IReader<TranslationRequest, TranslationResult>
    {
        private readonly HttpClient _client;
        private static readonly JsonSerializer _serializer = new JsonSerializer();
        private static readonly Dictionary<TranslationType, string> _urlPathMappings = new Dictionary<TranslationType, string>()
        {
            {TranslationType.Shakespeare, "shakespeare.json" },
            {TranslationType.Yoda, "yoda.json" },
        };


        public FunTranslationClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://api.funtranslations.com/translate/");
            _client.Timeout = new TimeSpan(0, 0, 30);
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<TranslationResult> Read(TranslationRequest request)
        {
            if (!_urlPathMappings.TryGetValue(request.Type, out string lastPartOfUrlPath))
                throw new NotSupportedException($"Fun translation of type {request.Type} is not supported");

            var urlParameters = new Dictionary<string, string>() { { "text", request.Text } };
            string urlPathWithQuery = QueryHelpers.AddQueryString(lastPartOfUrlPath, urlParameters);
            var uri = new Uri(urlPathWithQuery, UriKind.Relative);

            using (HttpResponseMessage response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                return stream.DeserializeFromJson<TranslationResult>(_serializer);
            }
        }
    }
}
