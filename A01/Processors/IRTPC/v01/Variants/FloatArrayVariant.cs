using System.IO;

namespace A01.Processors.IRTPC.v01.Variants
{
    public class FloatArrayVariant : PropertyVariants
    {
        public static int NUM = 2;
        public float[] Value;
    
        public FloatArrayVariant(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
        }

        public override void Serialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            for (int i = 0; i < NUM; i++)
            {
                bw.Write(Value[i]);
            }
        }
    
        public override void Deserialize(BinaryReader br)
        {
            Value = new float[NUM];
            for (int i = 0; i < NUM; i++)
            {
                Value[i] = br.ReadSingle();
            }
        }
    }
}