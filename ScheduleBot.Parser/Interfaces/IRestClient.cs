using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Interfaces
{
    public interface IRestClient
    {
        Task<string> SendAsync(HttpRequestMessage request);

        Task<string> GetAsync(Uri uri);

        Task<string> PostAsync(Uri uri, HttpContent content);
    }
}
