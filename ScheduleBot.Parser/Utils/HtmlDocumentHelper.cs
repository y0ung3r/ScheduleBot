using HtmlAgilityPack;

namespace ScheduleBot.Parser.Utils
{
    public static class HtmlDocumentHelper
    {
        public static HtmlDocument CreateNew(string html)
        {
            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(html);

            return htmlDocument;
        }
    }
}
