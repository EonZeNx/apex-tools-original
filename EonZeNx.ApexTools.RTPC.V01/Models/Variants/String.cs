using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class String : IPropertyVariants
    {
        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.String;
        public byte[] RawData { get; }
        public long Offset { get; set; }
        public uint Alignment => 0;
        public bool Primitive => false;

        public static readonly Dictionary<string, long> StringMap = new();

        public string Value;

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public String() { }
        public String(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }

        
        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(Encoding.UTF8.GetBytes(Value));
            bw.Write(0x00);
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
            List<byte> byteString = new List<byte>();
            while (true)
            {
                var thisByte = br.ReadByte();
                if (thisByte.Equals(0x00)) break;
                
                byteString.Add(thisByte);
            }
            Value = Encoding.UTF8.GetString(byteString.ToArray());
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
            Value = xr.ReadString();
        }

        public long MemorySerializeData(MemoryStream ms, long offset)
        {
            // If value already exists in file, use that offset
            if (StringMap.ContainsKey(Value))
            {
                Offset = StringMap[Value];
                return offset;
            }
            
            var coffset = ByteUtils.Align(ms, offset, Alignment);
            Offset = coffset;
            
            ms.Write(Encoding.UTF8.GetBytes(Value));
            ms.WriteByte(0x00);

            StringMap[Value] = Offset;

            return coffset + Value.Length + 1;
        }

        public void MemorySerializeHeader(MemoryStream ms)
        {
            ms.Write(BitConverter.GetBytes(NameHash));
            ms.Write(BitConverter.GetBytes((uint) Offset));
            ms.WriteByte((byte) VariantType);
        }
    }
}