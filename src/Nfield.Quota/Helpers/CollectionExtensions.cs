using System.Collections.Generic;
using System.Linq;

namespace Nfield.Quota.Helpers
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds range of items into collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Compares two definitions' collection if they have the same items and count
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool ScrambledDefinitionsEquals<T>(this ICollection<T> left, ICollection<T> right)
        {
            return left.Count == right.Count && left.All(right.Contains);
        }
    }
}
