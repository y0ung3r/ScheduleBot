using HtmlAgilityPack;
using ScheduleBot.Parser.Interfaces;
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

        public static async Task<HtmlDocument> CreateHtmlDocumentByGetAsync(this HttpClient httpClient, Uri uri)
        {
            var html = await GetAsync(httpClient, uri);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            return htmlDocument;
        }

        public static async Task<string> PostAsync(this HttpClient httpClient, Uri uri, HttpContent content)
        {
            var response = await httpClient.PostAsync(uri, content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<HtmlDocument> CreateHtmlDocumentByPostAsync(this HttpClient httpClient, Uri uri, HttpContent content)
        {
            var html = await PostAsync(httpClient, uri, content);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            return htmlDocument;
        }
    }
}
