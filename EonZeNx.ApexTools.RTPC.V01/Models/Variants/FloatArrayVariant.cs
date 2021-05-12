using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class FloatArrayVariant : IPropertyVariants
    {
        public SQLiteConnection DbConnection { get; set; }
        public string Name { get; set; } = "";

        public int NameHash { get; set; }
        private string HexNameHash => ByteUtils.IntToHex(NameHash);
        public virtual EVariantType VariantType { get; set; }
        public byte[] RawData { get; }
        public long Offset { get; set; }
        public virtual uint Alignment { get; set; }
        public bool Primitive => false;

        public virtual int NUM { get; set; } = 2;
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
            bw.Write(NameHash);
            bw.Write((uint) Offset);
            bw.Write((byte) VariantType);
        }
        
        public void BinarySerializeData(BinaryWriter bw)
        {
            ByteUtils.Align(bw, Alignment);
            Offset = bw.BaseStream.Position;
            
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
            
            // If valid connection, attempt to dehash
            if (DbConnection != null) Name = HashUtils.Lookup(DbConnection, NameHash);
        }

        public virtual void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameIfValid(xw, NameHash, Name);

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
            Value = Array.ConvertAll(floats, float.Parse);
        }

        public long MemorySerializeData(MemoryStream ms, long offset)
        {
            var coffset = ByteUtils.Align(ms, offset, Alignment);
            Offset = coffset;
            
            foreach (var val in Value)
            {
                ms.Write(BitConverter.GetBytes(val));
            }

            return coffset + (Value.Length * 4);
        }

        public void MemorySerializeHeader(MemoryStream ms)
        {
            ms.Write(BitConverter.GetBytes(NameHash));
            ms.Write(BitConverter.GetBytes((uint) Offset));
            ms.WriteByte((byte) VariantType);
        }
    }
}