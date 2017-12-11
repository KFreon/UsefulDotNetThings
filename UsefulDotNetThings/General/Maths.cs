using System;
using System.Collections.Generic;
using System.Text;

namespace UsefulDotNetThings.General
{
    public static class Maths
    {
        /// <summary>
        /// Counts the number of bits set in a uint.
        /// From here https://stackoverflow.com/questions/109023/how-to-count-the-number-of-set-bits-in-a-32-bit-integer
        /// </summary>
        /// <param name="i">Number to determine set bits in.</param>
        /// <returns>Number of bits set to 1.</returns>
        public static int CountSetBits(uint i)
        {
            i = i - ((i >> 1) & 0x5555_5555);
            i = (i & 0x3333_3333) + ((i >> 2) & 0x3333_3333);
            return (int)((((i + (i >> 4)) & 0x0F0F_0F0F) * 0x0101_0101) >> 24);
        }

        /// <summary>
        /// Determines if number is a power of 2. 
        /// </summary>
        /// <param name="number">Number to check.</param>
        /// <returns>True if number is a power of 2.</returns>
        public static bool IsPowerOfTwo(int number)
        {
            return (number & (number - 1)) == 0;
        }


        /// <summary>
        /// Determines if number is a power of 2. 
        /// </summary>
        /// <param name="number">Number to check.</param>
        /// <returns>True if number is a power of 2.</returns>
        public static bool IsPowerOfTwo(long number)
        {
            return (number & (number - 1)) == 0;
        }

        /// <summary>
        /// Rounds number to the nearest power of 2. Doesn't use Math. Uses bitshifting (not my method).
        /// </summary>
        /// <param name="number">Number to round.</param>
        /// <returns>Nearest power of 2.</returns>
        public static int RoundToNearestPowerOfTwo(int number)
        {
            // KFreon: Gets next Highest power
            int next = number - 1;
            next |= next >> 1;
            next |= next >> 2;
            next |= next >> 4;
            next |= next >> 8;
            next |= next >> 16;
            next++;

            // KFreon: Compare previous and next for the closest
            int prev = next >> 1;
            return number - prev > next - number ? next : prev;
        }
    }
}
