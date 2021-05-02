using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class UInt32 : IPropertyVariants
    {
        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.UInteger32;
        public byte[] RawData { get; }
        public uint Offset { get; }
        public uint Alignment => 0;
        public bool Primitive => true;
        
        public uint Value;

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public UInt32() { }
        public UInt32(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }


        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            Value = BitConverter.ToUInt32(RawData);
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");
            xw.WriteValue(Value);
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            Value = uint.Parse(xr.ReadString());
        }
    }
}