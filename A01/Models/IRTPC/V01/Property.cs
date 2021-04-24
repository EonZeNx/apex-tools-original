using System.IO;
using A01.Utils;

namespace A01.Models.IRTPC.V01
{
    public class Property
    {
        /* PROPERTY
         * Name hash : s32
         * Type : Enum
         */

        public long Offset { get; private set; }
        public int NameHash { get; private set; }
        public EVariantType Type { get; private set; }

        public Property(BinaryReader br)
        {
            Offset = BinaryReaderUtils.Position(br);
            NameHash = br.ReadInt32();
            Type = (EVariantType) br.ReadByte();
        }
    }
}