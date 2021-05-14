using System.Data.SQLite;
using System.IO;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.IRTPC.V01.Models
{
    public class Property
    {
        /* PROPERTY
         * Name hash : s32
         * Type : u8/Enum
         */

        public long Offset { get; private set; }
        public int NameHash { get; private set; }
        public EVariantType Type { get; private set; }
        
        public SQLiteConnection DbConnection { get; private set; }

        public Property(BinaryReader br, SQLiteConnection con = null)
        {
            Offset = BinaryReaderUtils.Position(br);
            NameHash = br.ReadInt32();
            Type = (EVariantType) br.ReadByte();

            DbConnection = con;
        }
    }
}