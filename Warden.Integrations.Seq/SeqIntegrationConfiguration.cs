using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Warden.Integrations.Seq
{
    /// <summary>
    /// Configuration of the SeqIntegration.
    /// </summary>
    public class SeqIntegrationConfiguration
    {
        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Include,
            Error = (serializer, error) => { error.ErrorContext.Handled = true; },
            Converters = new List<JsonConverter>
            {
                new Newtonsoft.Json.Converters.StringEnumConverter
                {
                    AllowIntegerValues = true,
                    CamelCaseText = true
                }
            }
        };

        /// <summary>
        /// Default request header name of the API key.
        /// </summary>
        public const string ApiKeyHeader = "X-Seq-ApiKey";

        public Dictionary<string, string> Headers { get; protected set; }

        /// <summary>
        /// URI of the Seq instance.
        /// </summary>
        public Uri Uri { get; protected set; }

        /// <summary>
        /// API key of Seq passed inside the custom "X-Api-Key" header.
        /// </summary>
        public string ApiKey { get; protected set; }

        /// <summary>
        /// Flag determining whether an exception should be thrown if PostAsync() returns invalid reponse (false by default).
        /// </summary>
        public bool FailFast { get; protected set; }

        /// <summary>
        /// Custom JSON serializer settings of the Newtonsoft.Json library.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; protected set; } = DefaultJsonSerializerSettings;

        /// <summary>
        /// Custom provider for the ISeqService.
        /// </summary>
        public Func<ISeqService> HttpServiceProvider { get; protected set; }

        /// <summary>
        /// Factory method for creating a new instance of fluent builder for the SeqIntegrationConfiguration.
        /// </summary>
        /// <param name="url">URL of the Seq instance (http://seq.example.com)</param>
        /// <param name="apiKey">Optional API key of Seq passed inside the custom "X-Seq-ApiKey" header.</param>
        /// <returns>Instance of fluent builder for the SeqIntegrationConfiguration.</returns>
        public static Builder Create(string url, string apiKey = null)
            => new Builder(url, apiKey);

        protected SeqIntegrationConfiguration(string url, string apiKey = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL can not be empty.", nameof(url));
            
            Uri = new Uri($"{url}/api/events/raw");
            HttpServiceProvider = () => new SeqService(new HttpClient());
            if(string.IsNullOrWhiteSpace(apiKey))
                return;

            ApiKey = apiKey;
            Headers = new Dictionary<string, string>()
            {
                {ApiKeyHeader, ApiKey }
            };
        }

        /// <summary>
        /// Fluent builder for the SeqIntegrationConfiguration.
        /// </summary>
        public class Builder
        {
            protected readonly SeqIntegrationConfiguration Configuration;

            public Builder(string url, string apiKey = null)
            {
                Configuration = new SeqIntegrationConfiguration(url, apiKey);
            }

            /// <summary>
            /// Sets the custom provider for the ISeqService.
            /// </summary>
            /// <param name="httpServiceProvider">Custom provider for the ISeqService.</param>
            /// <returns>Lambda expression returning an instance of the ISeqService.</returns>
            /// <returns>Instance of fluent builder for the SeqIntegrationConfiguration.</returns>
            public Builder WithHttpServiceProvider(Func<ISeqService> httpServiceProvider)
            {
                if (httpServiceProvider == null)
                    throw new ArgumentNullException(nameof(httpServiceProvider),
                        "HTTP service provider can not be null.");

                Configuration.HttpServiceProvider = httpServiceProvider;

                return this;
            }

            /// <summary>
            /// Flag determining whether an exception should be thrown if PostAsync() returns invalid reponse (false by default).
            /// </summary>
            /// <returns>Instance of fluent builder for the SeqIntegrationConfiguration.</returns>
            public Builder FailFast()
            {
                Configuration.FailFast = true;

                return this;
            }

            /// <summary>
            /// Builds the SeqIntegrationConfiguration and return its instance.
            /// </summary>
            /// <returns>Instance of SeqIntegrationConfiguration.</returns>
            public SeqIntegrationConfiguration Build() => Configuration;
        }
    }
}