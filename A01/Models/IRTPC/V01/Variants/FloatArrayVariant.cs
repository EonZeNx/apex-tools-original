using System.IO;

namespace A01.Models.IRTPC.V01.Variants
{
    public class FloatArrayVariant : PropertyVariants
    {
        public override int NameHash { get; set; }
        protected override EVariantType VariantType { get; set; }
        protected override long Offset { get; init; }
        
        protected static int NUM = 2;
        public float[] Value;

        /// <summary>
        /// Empty constructor for XML parsing.
        /// <see cref="FloatArrayVariant"></see>
        /// </summary>
        public FloatArrayVariant()
        {
            
        }
    
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