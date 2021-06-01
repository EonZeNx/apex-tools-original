using System;
using System.IO;

namespace EonZeNx.ApexTools.Core.Utils
{
    /// <summary>
    /// Extensions for Streams
    /// </summary>
    public static class StreamExtensions
    {
        #region Write Mantissa

        public static void Write(this Stream s, double value)
        {
            s.Write(BitConverter.GetBytes(value));
        }
        
        public static void Write(this Stream s, float value)
        {
            s.Write(BitConverter.GetBytes(value));
        }

        #endregion

        #region Write Signed

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
            // TODO: Write(Stream, sbyte) probably wrong? Look into this if needed in the future
            s.Write(BitConverter.GetBytes(value));
        }

        #endregion

        #region Write Unsigned

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
            s.WriteByte(value);
        }

        #endregion


        #region Read Mantissa

        public static double ReadDouble(this Stream s)
        {
            var value = new byte[8];
            s.Read(value, 0, 8);

            return BitConverter.ToDouble(value);
        }
        
        public static float ReadSingle(this Stream s)
        {
            var value = new byte[4];
            s.Read(value, 0, 4);

            return BitConverter.ToSingle(value);
        }

        #endregion

        #region Read Signed

        public static long ReadInt64(this Stream s)
        {
            var value = new byte[8];
            s.Read(value, 0, 8);

            return BitConverter.ToInt64(value);
        }
        
        public static int ReadInt32(this Stream s)
        {
            var value = new byte[4];
            s.Read(value, 0, 4);

            return BitConverter.ToInt32(value);
        }
        
        public static short ReadInt16(this Stream s)
        {
            var value = new byte[2];
            s.Read(value, 0, 2);

            return BitConverter.ToInt16(value);
        }
        
        public static sbyte ReadSByte(this Stream s)
        {
            return (sbyte) s.ReadByte();
        }

        #endregion

        #region Read Unsigned

        public static ulong ReadUInt64(this Stream s)
        {
            var value = new byte[8];
            s.Read(value, 0, 8);

            return BitConverter.ToUInt64(value);
        }
        
        public static uint ReadUInt32(this Stream s)
        {
            var value = new byte[4];
            s.Read(value, 0, 4);

            return BitConverter.ToUInt32(value);
        }
        
        public static ushort ReadUInt16(this Stream s)
        {
            var value = new byte[2];
            s.Read(value, 0, 2);

            return BitConverter.ToUInt16(value);
        }
        
        public static byte ReadUByte(this Stream s)
        {
            return (byte) s.ReadByte();
        }

        #endregion


        #region Read Multi

        public static byte[] ReadBytes(this Stream s, int count)
        {
            var value = new byte[count];
            s.Read(value, 0, count);

            return value;
        }

        #endregion
    }
}