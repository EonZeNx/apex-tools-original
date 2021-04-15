using System;
using System.Collections.Generic;

namespace A01.Utility
{
    public static class ConsoleUtils
    {
        public static string GetInput(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine();
        }
    }
    
    /// <summary>
    /// Custom ByteArray Equality Comparer.
    /// Arrays are references so two same arrays will not be the same.
    /// This allows byte arrays to be compared and return true if their contents are the same.
    /// </summary>
    public class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Hash code from here and here:
        /// https://stackoverflow.com/questions/14663168/an-integer-array-as-a-key-for-dictionary
        /// https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-overriding-gethashcode
        ///
        /// Basically, using two primes will give reasonable hashing.
        /// Extremely large primes are desired for larger amounts of hashes, however
        /// the use case for these byte arrays comparisons is only about 7 or 8 unique arrays.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(byte[] obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }
}