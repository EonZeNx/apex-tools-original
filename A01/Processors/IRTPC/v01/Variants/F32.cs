using System.IO;

namespace A01.Processors.IRTPC.v01.Variants
{
    public class F32 : PropertyVariants
    {
        public float Value;
        
        public F32(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            VariantType = EVariantType.Float32;
        }

        public override void Serialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            bw.Write(Value);
        }
        
        public override void Deserialize(BinaryReader br)
        {
            Value = br.ReadSingle();
        }
    }
}