using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class UIntArray : IPropertyVariants
    {
        public SQLiteConnection DbConnection { get; set; }
        public string Name { get; set; } = "";

        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.UInteger32Array;
        public byte[] RawData { get; }
        public long Offset { get; set; }
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
            bw.Write(NameHash);
            bw.Write((uint) Offset);
            bw.Write((byte) VariantType);
        }
        
        public void BinarySerializeData(BinaryWriter bw)
        {
            ByteUtils.Align(bw, Alignment);
            Offset = bw.BaseStream.Position;
            
            bw.Write(Value.Length);
            foreach (var u32 in Value)
            {
                bw.Write(u32);
            }
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
            
            // If valid connection, attempt to dehash
            if (DbConnection != null) Name = HashUtils.Lookup(DbConnection, NameHash);
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameIfValid(xw, NameHash, Name);

            var array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            
            var uintString = xr.ReadString();
            if (uintString.Length == 0)
            {
                Value = Array.Empty<uint>();
                return;
            }
            
            var uints = uintString.Split(",");
            Value = Array.ConvertAll(uints, uint.Parse);
        }
    }
}