using System.IO;
using System.Text;

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

        public static string ReadStringZ(this BinaryReader br)
        {
            var fullString = "";
            var character = "";
            
            while (character != "\0")
            {
                fullString += character;
                character = Encoding.UTF8.GetString(br.ReadBytes(1));
            }

            return fullString;
        }
        
        public static void WriteStringZ(this BinaryWriter bw, string value)
        {
            foreach (var character in value) { bw.Write(character); }

            if (value.EndsWith("\0")) return;
            
            bw.Write("\0");
        }
    }
}