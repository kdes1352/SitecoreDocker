using System.Collections.Generic;
using System.Linq;

namespace AllinaHealth.Framework.Extensions
{
    public static class EnumerableExtensions
    {
        public static void AddNonNull<T>(this List<T> list, T value)
        {
            if (value != null)
            {
                list.Add(value);
            }
        }

        public static List<List<T>> SplitList<T>(this List<T> l, int numberOfLists)
        {
            var list = new List<List<T>>();
            if (l.Count == 0)
            {
                return list;
            }

            var chunkSize = l.Count % numberOfLists;

            for (var i = 0; i < numberOfLists; i++)
            {
                list.Add(new List<T>());
                list[i].AddRange(l.Skip(i * chunkSize).Take(chunkSize));
            }

            return list;
        }
    }
}