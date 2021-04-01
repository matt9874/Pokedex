using Newtonsoft.Json;
using System;
using System.IO;

namespace Pokedex.Infrastructure.Extensions
{
    public static class StreamExtensions
    {
        public static T DeserializeFromJson<T>(this Stream stream, JsonSerializer serializer = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new NotSupportedException("Cannot read from this stream");

            serializer = serializer ?? new JsonSerializer();

            using (var streamReader = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }
    }
}
