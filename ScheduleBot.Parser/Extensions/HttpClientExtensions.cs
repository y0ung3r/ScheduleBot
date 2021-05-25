using HtmlAgilityPack;
using ScheduleBot.Parser.Utils;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<string> GetAsync(this HttpClient httpClient, Uri uri)
        {
            var response = await httpClient.GetAsync(uri);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PostAsync(this HttpClient httpClient, Uri uri, HttpContent content)
        {
            var response = await httpClient.PostAsync(uri, content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<HtmlDocument> CreateHtmlDocumentAsync(this HttpClient httpClient, Uri uri)
        {
            return HtmlDocumentHelper.CreateNew
            (
                await GetAsync(httpClient, uri)
            );
        }

        public static async Task<HtmlDocument> CreateHtmlDocumentAsync(this HttpClient httpClient, Uri uri, HttpContent content)
        {
            return HtmlDocumentHelper.CreateNew
            (
                await PostAsync(httpClient, uri, content)
            );
        }
    }
}
