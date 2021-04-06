using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Pokedex.Application.Translation.Dtos;
using Pokedex.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Translation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Pokedex.Application.Configuration;
using System.Linq;

namespace Pokedex.Infrastructure.WebRequests
{
    public abstract class FunTranslationClient: IReader<TranslationRequest, TranslationResult>
    {
        private static readonly string _httpClientName = "funTranslation";
        private static readonly string _httpClientConfigurationName = "funTranslationClient";

        private readonly HttpClient _client;
        private static readonly JsonSerializer _serializer = new JsonSerializer();
        protected abstract string LastPartOfUrlPath { get; }


        public FunTranslationClient(IHttpClientFactory clientFactory, IOptions<HttpClientOptions> options)
        {
            _client = clientFactory.CreateClient(_httpClientName);
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

        public async Task<TranslationResult> Read(TranslationRequest request)
        {
            string formattedText = RemoveIllegalCharacters(request.Text);

            var urlParameters = new Dictionary<string, string>() { { "text", formattedText } };
            string urlPathWithQuery = QueryHelpers.AddQueryString(LastPartOfUrlPath, urlParameters);
            var uri = new Uri(urlPathWithQuery, UriKind.Relative);

            using (HttpResponseMessage response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                return stream.DeserializeFromJson<TranslationResult>(_serializer);
            }
        }

        private string RemoveIllegalCharacters(string text)
        {
            return text.Replace('\r', ' ')
                .Replace('\n', ' ');
        }
    }
}
