using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using vFlash.Utils;

namespace vFlash.Utils
{

    /// <summary>
    /// Extension method for shuffling a list of type T.
    /// </summary>
    public static class Randomizer
    {
        public static IEnumerable<T> Shuffle1<T>(this IEnumerable<T> source)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            T[] copy = source.ToArray();

            for (int i = copy.Length - 1; i >= 0; i--)
            {
                int index = random.Next(i + 1);
                yield return copy[index];
                copy[index] = copy[i];
            }
        }
    }
}
