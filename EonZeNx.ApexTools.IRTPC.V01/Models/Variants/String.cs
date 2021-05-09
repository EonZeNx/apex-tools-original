using System.IO;
using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.IRTPC.V01.Models.Variants
{
    public class String : PropertyVariants
    {
        public override int NameHash { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.String;
        protected override long Offset { get; set; }
        public string Value;
        
        public String() { }
        public String(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
        }

        public override void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            bw.Write((ushort) Value.Length);
            bw.Write(Encoding.UTF8.GetBytes(Value));
        }
        
        public override void BinaryDeserialize(BinaryReader br)
        {
            var length = br.ReadUInt16();
            byte[] byteString = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byteString[i] = br.ReadByte();
            }
            Value = Encoding.UTF8.GetString(byteString);
        }

        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");
            xw.WriteValue(Value);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            Value = xr.ReadString();
        }
    }
}