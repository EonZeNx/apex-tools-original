﻿using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class F32Array : IPropertyVariants
    {
        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.Float32Array;
        public byte[] RawData { get; }
        public uint Offset { get; }
        public uint Alignment => 4;
        public bool Primitive => false;
        
        public float[] Value;

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public F32Array() { }
        public F32Array(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }
        
        
        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(Value.Length);
            foreach (var f32 in Value)
            {
                bw.Write(f32);
            }
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
            var length = br.ReadInt32();
            var values = new float[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = br.ReadSingle();
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
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            
            var floatString = xr.ReadString();
            if (floatString.Length == 0)
            {
                Value = Array.Empty<float>();
                return;
            }
            
            var floats = floatString.Split(",");
            Value = Array.ConvertAll(floats, float.Parse);
        }
    }
}