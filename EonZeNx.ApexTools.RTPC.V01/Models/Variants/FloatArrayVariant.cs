using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class FloatArrayVariant : IPropertyVariants
    {
        public int NameHash { get; set; }
        public EVariantType VariantType { get; set; }
        public byte[] RawData { get; }
        public uint Offset { get; }
        public uint Alignment => 0;
        public bool Primitive => false;

        public int NUM = 2;
        public float[] Value;
        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public FloatArrayVariant() { }
        public FloatArrayVariant(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }
        
        
        public void BinarySerialize(BinaryWriter bw)
        {
            foreach (var f32 in Value)
            {
                bw.Write(f32);
            }
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
            Value = new float[NUM];
            for (int i = 0; i < NUM; i++)
            {
                Value[i] = br.ReadSingle();
            }
        }

        public virtual void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");

            var array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public virtual void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            var floatString = xr.ReadString();
            var floats = floatString.Split(",");
            Value = Array.ConvertAll(floats, input => float.Parse(input));
        }
    }
}