using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.IRTPC.V01.Models.Variants
{
    public class UInt32 : PropertyVariants
    {
        public override int NameHash { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.UInteger32;
        protected override long Offset { get; set; }
        public uint Value;

        public UInt32() { }
        public UInt32(Property prop)
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
            Value = br.ReadUInt32();
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
            Value = uint.Parse(xr.ReadString());
        }
    }
}