using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Boticord.Net.Types;

namespace Boticord.Net
{
    public class BoticordNet
    {
        internal static HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.boticord.top/v1"),
            DefaultRequestHeaders =
            {
                Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
            }
        };

        internal readonly TimeSpan RateLimit = TimeSpan.FromSeconds(2);
        internal readonly TimeSpan BotsRateLimit = TimeSpan.FromSeconds(30);
        private DateTime _lastRequest;

        public BoticordNet(SdcConfig config)
        {
            // Wrapper = config.Wrapper;
            HttpClient = config.HttpClient ?? HttpClient;

            HttpClient.DefaultRequestHeaders.Add("Authorization", config.Token);
        }

        private async Task RateLimiter(TimeSpan timeout)
        {
            while (DateTime.UtcNow - _lastRequest <= timeout || _lastRequest != new DateTime())
                await Task.Delay(timeout - (DateTime.UtcNow - _lastRequest));

            _lastRequest = DateTime.UtcNow;
        }

        private async Task<T> Request<T>(Task<HttpResponseMessage> task, TimeSpan? timeout = null)
        {
            await RateLimiter(timeout ?? RateLimit);

            var response = await task;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;
        }

        internal async Task<T> PostRequest<T>(string path, StringContent data, TimeSpan? timeout = null) =>
            await Request<T>(HttpClient.PostAsync(path, data), timeout);

        internal async Task<T> GetRequest<T>(string path, TimeSpan? timeout = null) =>
            await Request<T>(HttpClient.GetAsync(path), timeout);
    }
}
