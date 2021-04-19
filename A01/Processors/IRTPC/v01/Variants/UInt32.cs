using System.IO;

namespace A01.Processors.IRTPC.v01.Variants
{
    public class UInt32 : PropertyVariants
    {
        public uint Value;
        
        public UInt32(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            VariantType = EVariantType.UInteger32;
        }

        public override void Serialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            bw.Write(Value);
        }
        
        public override void Deserialize(BinaryReader br)
        {
            Value = br.ReadUInt32();
        }
    }
}