using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class UIntArray : IPropertyVariants
    {
        public int NameHash { get; }
        public EVariantType VariantType => EVariantType.UInteger32Array;
        public byte[] RawData { get; }
        public uint Offset { get; }
        public uint Alignment => 4;
        public bool Primitive => false;
        
        public uint[] Value;

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public UIntArray() { }
        public UIntArray(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }
        
        
        public void BinarySerialize(BinaryWriter bw)
        {
            //
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
            var length = br.ReadInt32();
            var values = new uint[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = br.ReadUInt32();
            }

            Value = values;
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");

            string array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            //
        }
    }
}