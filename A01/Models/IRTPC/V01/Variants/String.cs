using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace A01.Models.IRTPC.V01.Variants
{
    public class String : PropertyVariants
    {
        [XmlAttribute]
        public override int NameHash { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.String;
        protected override long Offset { get; init; }
        public string Value;

        /// <summary>
        /// Empty constructor for XML parsing.
        /// <see cref="String"></see>
        /// </summary>
        public String()
        {
            
        }
        
        public String(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
        }

        public override void Serialize(BinaryWriter bw)
        {
            //
        }
        
        public override void Deserialize(BinaryReader br)
        {
            var length = br.ReadUInt16();
            byte[] byteString = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byteString[i] = br.ReadByte();
            }
            Value = Encoding.UTF8.GetString(byteString);
        }
    }
}