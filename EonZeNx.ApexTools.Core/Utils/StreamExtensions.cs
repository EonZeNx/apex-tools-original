using System;
using System.IO;

namespace EonZeNx.ApexTools.Core.Utils
{
    /// <summary>
    /// Extensions for Streams
    /// </summary>
    public static class StreamExtensions
    {
        #region Mantissa

        public static void Write(this Stream s, double value)
        {
            s.Write(BitConverter.GetBytes(value));
        }
        
        public static void Write(this Stream s, float value)
        {
            s.Write(BitConverter.GetBytes(value));
        }

        #endregion

        #region Signed

        public static void Write(this Stream s, long value)
        {
            s.Write(BitConverter.GetBytes(value));
        }
        
        public static void Write(this Stream s, int value)
        {
            s.Write(BitConverter.GetBytes(value));
        }
        
        public static void Write(this Stream s, short value)
        {
            s.Write(BitConverter.GetBytes(value));
        }
        
        public static void Write(this Stream s, sbyte value)
        {
            s.Write(BitConverter.GetBytes(value));
        }

        #endregion

        #region Unsigned

        public static void Write(this Stream s, ulong value)
        {
            s.Write(BitConverter.GetBytes(value));
        }
        
        public static void Write(this Stream s, uint value)
        {
            s.Write(BitConverter.GetBytes(value));
        }

        public static void Write(this Stream s, ushort value)
        {
            s.Write(BitConverter.GetBytes(value));
        }

        public static void Write(this Stream s, byte value)
        {
            s.Write(BitConverter.GetBytes(value));
        }

        #endregion
    }
}