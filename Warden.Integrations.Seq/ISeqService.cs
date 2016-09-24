using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Warden.Integrations.Seq
{
    /// <summary>
    /// Custom Seq client for executing the POST request.
    /// </summary>
    public interface ISeqService
    {
        /// <summary>
        /// Executes the Seq POST request.
        /// </summary>
        /// <param name="url">Full API URL (base + endpoint) of the request (e.g. http://www.my-api.com)</param>
        /// <param name="data">Request data in JSON format that will be sent.</param>
        /// <param name="headers">Optional request headers.</param>
        /// <param name="failFast">Flag determining whether an exception should be thrown if received reponse is invalid (false by default).</param>
        /// <returns>Instance of IHttpResponse.</returns>
        Task PostAsync(string url, string data, IDictionary<string, string> headers = null,
            bool failFast = false);
    }

    /// <summary>
    /// Default implementation of the ISeqService based on SeqService.
    /// </summary>
    public class SeqService : ISeqService
    {
        private readonly HttpClient _client;

        public SeqService(HttpClient client)
        {
            _client = client;
        }

        public async Task PostAsync(string url, string data, IDictionary<string, string> headers = null,
            bool failFast = false)
        {
            SetRequestHeaders(headers);
            try
            {
                var response = await _client.PostAsync(url, new StringContent(
                    data, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                    return;
                if (!failFast)
                    return;

                throw new Exception($"Received invalid HTTP response with status code: {response.StatusCode}. " +
                    $"Reason phrase: {response.ReasonPhrase}");
            }
            catch (Exception exception)
            {
                if (!failFast)
                    return;

                throw new Exception($"There was an error while executing the PostAsync(): " +
                                    $"{exception}", exception);
            }
        }

        private void SetRequestHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
                return;

            foreach (var header in headers)
            {
                var existingHeader = _client.DefaultRequestHeaders
                    .FirstOrDefault(x => string.Equals(x.Key, header.Key, StringComparison.CurrentCultureIgnoreCase));
                if (existingHeader.Key != null)
                    _client.DefaultRequestHeaders.Remove(existingHeader.Key);

                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
    }
}