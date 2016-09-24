using System;
using System.Threading.Tasks;

namespace Warden.Integrations.Seq
{
    /// <summary>
    /// Integration with Seq for sending information about performed checks.
    /// </summary>
    public class SeqIntegration : IIntegration
    {
        private readonly SeqIntegrationConfiguration _configuration;

        public SeqIntegration(SeqIntegrationConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration),
                    "Seq Integration configuration has not been provided.");
            }

            _configuration = configuration;
        }

        /// <summary>
        /// Posts the iteration to Seq
        /// </summary>
        /// <param name="iteration">Iteration object that will be serialized to the JSON.</param>
        public async Task PostIterationToSeqAsync(IWardenIteration iteration)
        {
            if (iteration == null)
                throw new ArgumentNullException(nameof(iteration), "Warden iteration can not be null.");
            if (string.IsNullOrWhiteSpace(iteration.WardenName))
                throw new ArgumentException("Warden name can not be empty.", nameof(iteration.WardenName));
            await _configuration.HttpServiceProvider().PostAsync(_configuration.Uri.ToString(),
                iteration.ToJson(_configuration.JsonSerializerSettings), _configuration.Headers,
                _configuration.FailFast);
        }

        /// <summary>
        /// Factory method for creating a new instance of SeqIntegration.
        /// </summary>
        /// <param name="url">URL of the Seq instance.</param>
        /// <param name="apiKey">Optional API key of the Seq instance passed inside the custom "X-Api-Key" header.</param>
        /// <param name="configurator">Lambda expression for configuring the SeqIntegration integration.</param>
        /// <returns>Instance of SeqIntegration.</returns>
        public static SeqIntegration Create(string url, string apiKey = null,
            Action<SeqIntegrationConfiguration.Builder> configurator = null)
        {
            var config = new SeqIntegrationConfiguration.Builder(url, apiKey);
            configurator?.Invoke(config);

            return Create(config.Build());
        }

        /// <summary>
        /// Factory method for creating a new instance of SeqIntegration.
        /// </summary>
        /// <param name="configuration">Configuration of Seq integration.</param>
        /// <returns>Instance of SeqIntegration.</returns>
        public static SeqIntegration Create(SeqIntegrationConfiguration configuration)
            => new SeqIntegration(configuration);
    }
}