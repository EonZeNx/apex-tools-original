using System.IO;
using System.Xml.Serialization;

namespace A01.Models.IRTPC.V01.Variants
{
    public class F32 : PropertyVariants
    {
        [XmlAttribute]
        public override int NameHash { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.Float32;
        protected override long Offset { get; init; }
        public float Value;

        /// <summary>
        /// Empty constructor for XML parsing.
        /// <see cref="F32"></see>
        /// </summary>
        public F32()
        {
            
        }
        
        public F32(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
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