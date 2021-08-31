using ScheduleBot.Parser.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScheduleBot.Parser
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _httpClient;

        public RestClient()
        {
            _httpClient = new HttpClient();
        }

        public RestClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SendAsync(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public Task<string> GetAsync(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            return SendAsync(request);
        }

        public Task<string> PostAsync(Uri uri, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };

            return SendAsync(request);
        }
    }
}
