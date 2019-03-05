using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquinoxLabs.SVGSharpie.DynamicPDF.Extensions
{
    internal static class CollectionExtensions
    {
        /// <summary>
        /// Count from the back of the collection until the predicate returns true
        /// </summary>
        /// <returns>number of items at the back of the collection which fail the specified predicate check</returns>
        public static int CountReverseUntil<T>(this IReadOnlyList<T> array, Predicate<T> predicate)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            for (var i = array.Count - 1; i >= 0; i--)
            {
                if (predicate(array[i]))
                {
                    return array.Count - (i + 1);
                }
            }
            return array.Count;
        }

        /// <summary>
        /// Calls the <see cref="ICollection{T}.Add"/> method foreach of the items in the specified sequence
        /// </summary>
        /// <param name="collection">the collection to add items to</param>
        /// <param name="items">the items to add to the collection</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (items == null) throw new ArgumentNullException(nameof(items));
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
