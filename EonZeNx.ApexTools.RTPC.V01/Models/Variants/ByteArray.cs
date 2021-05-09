using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class ByteArray : IPropertyVariants
    {
        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.ByteArray;
        public byte[] RawData { get; }
        public long Offset { get; set; }
        public uint Alignment => 4;
        public bool Primitive => false;
        
        public byte[] Value;

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public ByteArray() { }
        public ByteArray(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }
        
        
        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((uint) Offset);
            bw.Write((byte) VariantType);
        }
        
        public void BinarySerializeData(BinaryWriter bw)
        {
            ByteUtils.Align(bw, Alignment);
            Offset = bw.BaseStream.Position;
            
            bw.Write(Value.Length);
            foreach (var val in Value)
            {
                bw.Write(val);
            }
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
            var length = br.ReadInt32();
            if (length != 0)
            {
                Value = br.ReadBytes(length);
            }
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");

            var array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            var floatString = xr.ReadString();
            var floats = floatString.Split(",");
            Value = Array.ConvertAll(floats, input => byte.Parse(input));
        }
    }
}