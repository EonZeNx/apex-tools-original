using System;
using System.Globalization;

namespace A01.Utils
{
    public static class HexUtils
    {
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
    }
}