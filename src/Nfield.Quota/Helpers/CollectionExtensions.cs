using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
