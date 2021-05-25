using System.Data.SQLite;
using System.IO;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models
{
    /// <summary>
    /// Shared <see cref="Property"/> for PropertyVariants
    /// <br/> Structure:
    /// <br/> Name hash - <see cref="int"/>
    /// <br/> Raw data - <see cref="uint"/>
    /// <br/> Type - <see cref="sbyte"/>/<see cref="EVariantType"/>
    /// </summary>
    public class Property
    {
        public uint Offset { get; private set; }
        public int NameHash { get; private set; }
        public byte[] RawData { get; private set; }
        public EVariantType Type { get; private set; }
        
        public SQLiteConnection DbConnection { get; private set; }

        public Property(BinaryReader br, SQLiteConnection con = null)
        {
            Offset = (uint) BinaryReaderUtils.Position(br);
            NameHash = br.ReadInt32();
            RawData = br.ReadBytes(4);
            Type = (EVariantType) br.ReadByte();

            DbConnection = con;
        }
    }
}