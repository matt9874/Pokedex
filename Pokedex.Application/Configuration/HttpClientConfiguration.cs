using System;

namespace Pokedex.Application.Configuration
{
    public class HttpClientConfiguration
    {
        public string Name { get; set; }
        public string BaseAddress { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}