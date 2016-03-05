using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public static class Util
    {
        /// <summary>
        /// Returns a random boolean array with the specified number of true and false entries.
        /// </summary>
        /// <param name="countTrue"></param>
        /// <param name="countFalse"></param>
        /// <returns></returns>
        public static Boolean[] PickRandom(int countTrue, int countFalse)
        {
            Boolean[] of_the_king = null;
            Util.PickRandom(countTrue, countFalse, ref of_the_king);
            return of_the_king;
        }

        /// <summary>
        /// Sets sent boolean array with the specified number of true and false entries.
        /// </summary>
        /// <param name="countTrue"></param>
        /// <param name="countFalse"></param>
        /// <returns></returns>
        public static void PickRandom(int countTrue, int countFalse, ref Boolean[] b)
        {
            bool remake = true;

            if (b != null)
                if (b.Length == countFalse + countTrue)
                    remake = false;

            if (remake) b = new Boolean[countFalse + countTrue];

            bool swap = countTrue > countFalse;

            for (int i = 0; i < b.Length; i++)
                b[i] = swap;

            int min = Math.Min(countTrue, countFalse);

            Random r = new Random();

            while (min > 0)
            {
                int i = r.Next() % b.Length;
                if (b[i] == swap)
                {
                    b[i] = !swap;
                    min--;
                }
            }
        }
    }
}
