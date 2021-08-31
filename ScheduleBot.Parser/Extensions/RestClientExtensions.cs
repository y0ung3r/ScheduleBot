using HtmlAgilityPack;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Parser.Utils;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Extensions
{
    public static class RestClientExtensions
    {
        public static async Task<HtmlDocument> CreateHtmlDocumentAsync(this IRestClient restClient, Uri uri)
        {
            return HtmlDocumentHelper.CreateNew
            (
                await restClient.GetAsync(uri)
            );
        }

        public static async Task<HtmlDocument> CreateHtmlDocumentAsync(this IRestClient restClient, Uri uri, HttpContent content)
        {
            return HtmlDocumentHelper.CreateNew
            (
                await restClient.PostAsync(uri, content)
            );
        }
    }
}
