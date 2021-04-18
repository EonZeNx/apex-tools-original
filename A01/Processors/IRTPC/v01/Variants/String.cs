using System.IO;
using System.Text;

namespace A01.Processors.IRTPC.v01.Variants
{
    public class String : PropertyVariants
    {
        public string Value;
        
        public String(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            VariantType = EVariantType.String;
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