using System.IO;

namespace EonZeNx.ApexTools.Core.Utils
{
    public static class BinaryReaderUtils
    {
        public static long Position(BinaryReader reader)
        {
            return reader.BaseStream.Position;
        }
    }
}