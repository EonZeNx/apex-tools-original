using System.IO;
using System.Xml;
using A01.Utils;

namespace A01.Models.IRTPC.V01.Variants
{
    public class F32 : PropertyVariants
    {
        public override int NameHash { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.Float32;
        protected override long Offset { get; set; }
        public float Value;
        
        public F32() { }
        public F32(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
        }

        public override void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            bw.Write(Value);
        }
        
        public override void BinaryDeserialize(BinaryReader br)
        {
            Value = br.ReadSingle();
        }

        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{HexUtils.IntToHex(NameHash)}");
            xw.WriteValue(Value);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = HexUtils.HexToInt(nameHash);
            Value = float.Parse(xr.ReadString());
        }
    }
}