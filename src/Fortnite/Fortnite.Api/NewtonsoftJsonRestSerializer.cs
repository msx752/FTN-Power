using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fortnite.Api
{
    public class NewtonsoftJsonRestSerializer : IRestSerializer
    {
        private readonly JsonSerializer _serializer;

        public NewtonsoftJsonRestSerializer()
        {
            _serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    _serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        public string ContentType { get; set; } = "application/json";

        public T Deserialize<T>(IRestResponse response)
        {
            var content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return _serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public string Serialize(Parameter parameter) => Serialize(parameter.Value);

        public string[] SupportedContentTypes { get; } =
        {
        "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
    };

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}
