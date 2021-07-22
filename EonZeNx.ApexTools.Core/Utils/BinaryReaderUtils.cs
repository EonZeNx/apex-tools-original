using System.IO;

namespace EonZeNx.ApexTools.Core.Utils
{
    public static class BinaryReaderUtils
    {
        public static long Position(BinaryReader reader)
        {
            return reader.BaseStream.Position;
        }

        public static byte[] StreamToBytes(Stream s)
        {
            s.Seek(0, SeekOrigin.Begin);
            var contents = new byte[s.Length];
            s.Read(contents, 0, (int) s.Length);

            return contents;
        }
    }
}