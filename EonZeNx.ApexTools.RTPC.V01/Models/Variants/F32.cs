using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class F32 : IPropertyVariants
    {
        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.Float32;
        public byte[] RawData { get; }
        public long Offset { get; }
        public uint Alignment => 0;
        public bool Primitive => true;
        
        public float Value;

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public F32() { }
        public F32(Property prop)
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
            Value = BitConverter.ToSingle(RawData);
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
            Value = float.Parse(xr.ReadString());
        }

        public long MemorySerializeData(MemoryStream ms, long offset)
        {
            // Is primitive, does not need memory serialize
            return offset;
        }

        public void MemorySerializeHeader(MemoryStream ms)
        {
            ms.Write(BitConverter.GetBytes(NameHash));
            ms.Write(BitConverter.GetBytes(Value));
            ms.WriteByte((byte) VariantType);
        }
    }
}