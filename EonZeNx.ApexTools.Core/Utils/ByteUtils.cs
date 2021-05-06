using System;
using System.Globalization;
using System.IO;

namespace EonZeNx.ApexTools.Core.Utils
{
    public static class ByteUtils
    {
        #region Hex

        public static string UintToHex(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            var returnValue = "";
            foreach (byte b in bytes)
                returnValue += b.ToString("X2");
            return returnValue;
        }
        
        public static string IntToHex(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            var returnValue = "";
            foreach (byte b in bytes)
                returnValue += b.ToString("X2");
            return returnValue;
        }
        
        public static string LongToHex(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            var returnValue = "";
            foreach (byte b in bytes)
                returnValue += b.ToString("X2");
            return returnValue;
        }
        
        public static string UlongToHex(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            var returnValue = "";
            foreach (byte b in bytes)
                returnValue += b.ToString("X2");
            return returnValue;
        }
        
        public static int HexToInt(string value)
        {
            if (value.Length < 1) return 0;

            string reversedValue = "";
            for (int i = value.Length - 2; i >= 0; i -= 2)
            {
                reversedValue += value[i..(i + 2)];
            }
            
            return int.Parse(reversedValue, NumberStyles.AllowHexSpecifier);
        }
        
        public static uint HexToUint(string value)
        {
            if (value.Length < 1) return 0;

            string reversedValue = "";
            for (int i = value.Length - 2; i >= 0; i -= 2)
            {
                reversedValue += value[i..(i + 2)];
            }
            
            return uint.Parse(reversedValue, NumberStyles.AllowHexSpecifier);
        }

        #endregion

        #region Bytes

        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }
        
        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
        
        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        #endregion

        #region Alignment

        public static int Align(int value, int align)
        {
            if (value == 0) return align;
            if (align == 0) return value;
            
            return value + ((align - (value % align)) % align);
        }
        
        public static uint Align(uint value, uint align)
        {
            if (value == 0) return align;
            if (align == 0) return value;

            return value + ((align - (value % align)) % align);
        }
        
        public static long Align(long value, long align)
        {
            if (value == 0) return 0;
            if (align == 0) return value;

            return value + ((align - (value % align)) % align);
        }
        
        public static ulong Align(ulong value, ulong align)
        {
            if (value == 0) return align;
            if (align == 0) return value;

            return value + ((align - (value % align)) % align);
        }

        public static void Align(BinaryReader br, uint align)
        {
            br.BaseStream.Seek(Align(br.BaseStream.Position, align), SeekOrigin.Begin);
        }
        
        public static long Align(MemoryStream ms, long offset, long align)
        {
            var preAlign = Align(offset, align);
            var alignment = preAlign - offset;
            for (int i = 0; i < alignment; i++)
            {
                ms.WriteByte(0x50);
            }

            return preAlign;
        }

        #endregion
    }
}