using HtmlAgilityPack;
using System;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Extensions
{
    /// <summary>
    /// Определяет методы-расширения, которые позволяют продолжать асинхронную цепочку вызовов совместно с Flurl
    /// /
    /// Defines extension methods that allow the asynchronous call chain to continue along with Flurl
    /// </summary>
    public static class FlurlTaskExtensions
    {
        /// <summary>
        /// Выполняет задачу по получению текстового содержимого и на его основе формирует HTML-документ
        /// /
        /// Perform <see cref="Task"/> to get a string and use that result to create an HTML document
        /// </summary>
        /// <param name="stringReceivingTask">Задача по получению текстового содержимого / String receiving task</param>
        /// <returns>Готовый HTML-документ / HTML document</returns>
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
