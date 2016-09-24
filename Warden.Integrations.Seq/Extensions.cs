using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Warden.Core;

namespace Warden.Integrations.Seq
{
    /// <summary>
    /// Custom extension methods for the Seq integration.
    /// </summary>
    public static class Extensions
    {
        internal static string ToSeqJson(this IWardenIteration data, JsonSerializerSettings serializerSettings)
        {
            var rawData = new {
                Events = new List<object>()
            };
            foreach (IWardenCheckResult checkResult in data.Results)
            {
                var @event = checkResult.ToSeqObject(data.WardenName);
                rawData.Events.Add(@event);
            }
            return JsonConvert.SerializeObject(rawData, serializerSettings);
        }

        internal static object ToSeqObject(this IWardenCheckResult checkResult, string wardenName = null)
        {
            string messageTemplate = "Watcher {WatcherName} ran with result {Description}";
            if (checkResult.Exception != null)
            {
                messageTemplate = "Watcher {WatcherName} ran but encountered an exception {Exception}";
            }

            var @event = new
            {
                Timestamp = checkResult.CompletedAt,
                Level = checkResult.IsValid ? "Debug" : "Error",
                MessageTemplate = messageTemplate,
                Properties = new
                {
                    WatcherName = checkResult.WatcherCheckResult.WatcherName,
                    Warden = wardenName,
                    WatcherGroup = checkResult.WatcherCheckResult.WatcherGroup,
                    WatcherType = checkResult.WatcherCheckResult.WatcherType,
                    Description = checkResult.WatcherCheckResult.Description,
                    Exception = checkResult.Exception,
                    StartedAt = checkResult.StartedAt,
                    CompletedAt = checkResult.CompletedAt,
                    ExecutionTime = checkResult.ExecutionTime
                }
            };
            return @event;
        }

        internal static string ToSeqJson(this IWardenCheckResult checkResult, JsonSerializerSettings serializerSettings)
        {
            var rawData = new
            {
                Events = new List<object>()
            };
            rawData.Events.Add(checkResult.ToSeqObject());
            return JsonConvert.SerializeObject(rawData, serializerSettings);
        }

        /// <summary>
        /// Extension method for adding the Seq integration to the the WardenConfiguration.
        /// </summary>
        /// <param name="builder">Instance of the Warden configuration builder.</param>
        /// <param name="url">URL of the Seq instance.</param>
        /// <param name="configurator">Optional lambda expression for configuring the SeqIntegration.</param>
        public static WardenConfiguration.Builder IntegrateWithSeq(
            this WardenConfiguration.Builder builder,
            string url,
            Action<SeqIntegrationConfiguration.Builder> configurator = null)
        {
            builder.AddIntegration(SeqIntegration.Create(url, configurator: configurator));

            return builder;
        }

        /// <summary>
        /// Extension method for adding the Seq integration to the the WardenConfiguration.
        /// </summary>
        /// <param name="builder">Instance of the Warden configuration builder.</param>
        /// <param name="url">URL of the Seq instance.</param>
        /// <param name="apiKey">API key of Seq passed inside the custom "X-Seq-ApiKey" header.</param>
        /// <param name="configurator">Optional lambda expression for configuring the SeqIntegration.</param>
        public static WardenConfiguration.Builder IntegrateWithSeq(
            this WardenConfiguration.Builder builder,
            string url, string apiKey,
            Action<SeqIntegrationConfiguration.Builder> configurator = null)
        {
            builder.AddIntegration(SeqIntegration.Create(url, apiKey, configurator: configurator));

            return builder;
        }

        /// <summary>
        /// Extension method for adding the Seq integration to the the WardenConfiguration.
        /// </summary>
        /// <param name="builder">Instance of the Warden configuration builder.</param>
        /// <param name="configuration">Configuration of SeqIntegration.</param>
        /// <returns>Instance of fluent builder for the WardenConfiguration.</returns>
        public static WardenConfiguration.Builder IntegrateWithSeq(
            this WardenConfiguration.Builder builder,
            SeqIntegrationConfiguration configuration)
        {
            builder.AddIntegration(SeqIntegration.Create(configuration));

            return builder;
        }

        /// <summary>
        /// Extension method for resolving the Seq integration from the IIntegrator.
        /// </summary>
        /// <param name="integrator">Instance of the IIntegrator.</param>
        /// <returns>Instance of fluent builder for the WardenConfiguration.</returns>
        public static SeqIntegration Seq(this IIntegrator integrator)
            => integrator.Resolve<SeqIntegration>();
    }
}