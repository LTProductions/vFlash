using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using vFlash.Utils;

namespace vFlash.Utils
{
    public static class Randomizer
    {
        public static IEnumerable<T> Shuffle1<T>(this IEnumerable<T> source)
        {
            Random random = new Random();
            T[] copy = source.ToArray();

            for (int i = copy.Length - 1; i >= 0; i--)
            {
                int index = random.Next(i + 1);
                yield return copy[index];
                copy[index] = copy[i];
            }
        }


        public static int RandomNumber()
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            return 1;
        }
    }
}
