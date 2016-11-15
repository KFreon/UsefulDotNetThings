using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    /// <summary>
    /// Performs bit operations like BitConverter, however allowing interpreting bytes as Big Endian.
    /// Uses the MS BitConverter, also all descriptions come from: https://msdn.microsoft.com/en-us/library/system.bitconverter(v=vs.110).aspx
    /// </summary>
    public static class MyBitConverter
    {
        /// <summary>
        /// Indicates the endianess of the current machine.
        /// </summary>
        public static bool IsMachineLittleEndian = BitConverter.IsLittleEndian;

        /// <summary>
        /// Allows BitConverter operations to be interpreted as Big Endian.
        /// True = interpret as Little Endian (default)
        /// False = interpret as Big Endian (Network Byte Order)
        /// </summary>
        public static bool AreOperationsLittleEndian { get; set; } = BitConverter.IsLittleEndian;

        /// <summary>
        /// Converts the specified double-precision floating point number to a 64-bit signed integer.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>A 64-bit signed integer whose value is equivalent to value.</returns>
        public static long DoubleToInt64Bits(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        /// <summary>
        /// Converts the specified 64-bit signed integer to a double-precision floating point number.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>A double-precision floating point number whose value is equivalent to value.</returns>
        public static double Int64BitsToDouble(long value)
        {
            return BitConverter.Int64BitsToDouble(value);
        }

        #region GetBytes
        /// <summary>
        /// Returns the specified Boolean value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// Returns the specified Unicode character value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(char value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// Returns the specified double-precision floating point value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!AreOperationsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 16 bit signed value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!AreOperationsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 32 bit signed value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!AreOperationsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 64 bit signed value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!AreOperationsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified single-precision floating point value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(Single value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!AreOperationsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 16 bit unsigned value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!AreOperationsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 32 bit unsigned value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!AreOperationsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 64 bit unsigned value as a byte array.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        public static byte[] GetBytes(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!AreOperationsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }
        #endregion GetBytes

        #region To<type>
        /// <summary>
        /// Returns a Boolean value converted from the byte at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static bool ToBoolean(byte[] source, int startIndex)
        {
            return BitConverter.ToBoolean(source, startIndex);
        }

        /// <summary>
        /// Returns a Unicode character converted from the byte at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static char ToChar(byte[] source, int startIndex)
        {
            return BitConverter.ToChar(source, startIndex);
        }

        /// <summary>
        /// Returns a double-precision floating point value converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static double ToDouble(byte[] source, int startIndex)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<double>(source, startIndex);
                return BitConverter.ToDouble(bytes, 0);
            }
            else
                return BitConverter.ToDouble(source, startIndex);
            
        }

        /// <summary>
        /// Returns a 16 bit signed value converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static short ToInt16(byte[] source, int startIndex)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<short>(source, startIndex);
                return BitConverter.ToInt16(bytes, 0);
            }
            else
                return BitConverter.ToInt16(source, startIndex);
        }


        /// <summary>
        /// Returns a 32 signed value converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static int ToInt32(byte[] source, int startIndex)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<int>(source, startIndex);
                return BitConverter.ToInt32(bytes, 0);
            }
            else
                return BitConverter.ToInt32(source, startIndex);
        }

        /// <summary>
        /// Returns a 64 bit signed value converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static long ToInt64(byte[] source, int startIndex)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<long>(source, startIndex);
                return BitConverter.ToInt64(bytes, 0);
            }
            else
                return BitConverter.ToInt64(source, startIndex);
        }

        /// <summary>
        /// Returns a single-precision floating point value converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static float ToSingle(byte[] source, int startIndex)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<float>(source, startIndex);
                return BitConverter.ToSingle(bytes, 0);
            }
            else
                return BitConverter.ToSingle(source, startIndex);
        }

        /// <summary>
        /// Converts the numeric value of each element of a specified array of bytes to its equivalent hexadecimal string representation.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static string ToString(byte[] source, int startIndex)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<string>(source, startIndex);
                return BitConverter.ToString(bytes, 0);
            }
            else
                return BitConverter.ToString(source, startIndex);
        }

        /// <summary>
        /// Converts the numeric value of each element of a specified array of bytes to its equivalent hexadecimal string representation.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        /// <param name="length">Number of bytes to convert to string.</param>
        public static string ToString(byte[] source, int startIndex, int length)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<string>(source, startIndex);
                return BitConverter.ToString(bytes, 0, length);
            }
            else
                return BitConverter.ToString(source, startIndex, length);
        }

        /// <summary>
        /// Converts the numeric value of each element of a specified array of bytes to its equivalent hexadecimal string representation.
        /// </summary>
        /// <param name="source">A byte array.</param>
        public static string ToString(byte[] source)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<string>(source, 0);
                return BitConverter.ToString(bytes);
            }
            else
                return BitConverter.ToString(source);
        }

        /// <summary>
        /// Returns a 16 bit unsigned value converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static ushort ToUInt16(byte[] source, int startIndex)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<short>(source, startIndex);
                return BitConverter.ToUInt16(bytes, 0);
            }
            else
                return BitConverter.ToUInt16(source, startIndex);
        }

        /// <summary>
        /// Returns a 32 bit unsigned value converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static uint ToUInt32(byte[] source, int startIndex)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<int>(source, startIndex);
                return BitConverter.ToUInt32(bytes, 0);
            }
            else
                return BitConverter.ToUInt32(source, startIndex);
        }

        /// <summary>
        /// Returns a 64 bit unsigned value converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="source">A byte array.</param>
        /// <param name="startIndex">The index of the byte within value.</param>
        public static ulong ToUInt64(byte[] source, int startIndex)
        {
            if (!AreOperationsLittleEndian)
            {
                var bytes = GetAndReverseBytes<long>(source, startIndex);
                return BitConverter.ToUInt64(bytes, 0);
            }
            else
                return BitConverter.ToUInt64(source, startIndex);
        }
        #endregion To<type>

        static byte[] GetAndReverseBytes<T>(byte[] source, int position, int length = 0)
        {
            int numBytes = 0;

            var type = typeof(T);
            if (type == typeof(int))
                numBytes = 4;
            else if (type == typeof(short))
                numBytes = 2;
            else if (type == typeof(double))
                numBytes = 8;
            else if (type == typeof(long))
                numBytes = 8;
            else if (type == typeof(float))
                numBytes = 4;
            else if (type == typeof(string))
                numBytes = length == 0 ? source.Length - position : length;

            byte[] bytes = new byte[numBytes];
            Array.Copy(source, position, bytes, 0, numBytes);
            Array.Reverse(bytes);
            return bytes;
        }
    }
}
