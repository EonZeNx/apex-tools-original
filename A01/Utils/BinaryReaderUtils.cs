using System.IO;

namespace A01.Utils
{
    public static class BinaryReaderUtils
    {
        public static long Position(BinaryReader reader)
        {
            return reader.BaseStream.Position;
        }
    }
}