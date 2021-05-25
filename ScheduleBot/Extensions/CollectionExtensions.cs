using System.Collections.Generic;
using System.Linq;

namespace ScheduleBot.Extensions
{
    public static class CollectionExtensions
    {
        public static IList<IList<TSource>> ChunkBy<TSource>(this ICollection<TSource> source, int columnsCount)
        {
            return source.Select((element, index) => new 
                         { 
                             Index = index, 
                             Value = element
                         })
                         .GroupBy(element => element.Index / columnsCount)
                         .Select
                         (
                             element => element.Select(x => x.Value).ToList() as IList<TSource>
                         )
                         .ToList();
        }
    }
}
