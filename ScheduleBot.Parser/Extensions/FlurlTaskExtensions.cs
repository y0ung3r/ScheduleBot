using HtmlAgilityPack;
using System;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Extensions
{
    public static class FlurlTaskExtensions
    {
        public static async Task<HtmlDocument> ToHtmlDocumentAsync(this Task<string> stringReceivingTask)
        {
            if (stringReceivingTask is null)
            {
                throw new ArgumentNullException(nameof(stringReceivingTask));
            }

            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml
            (
                await stringReceivingTask
            );

            return htmlDocument;
        }
    }
}
