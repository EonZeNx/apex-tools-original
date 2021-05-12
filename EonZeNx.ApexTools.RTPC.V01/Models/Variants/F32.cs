using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class F32 : IPropertyVariants
    {
        public SQLiteConnection DbConnection { get; set; }
        public string Name { get; set; } = "";

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
            DbConnection = prop.DbConnection;
        }

        
        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write(Value);
            bw.Write((byte) VariantType);
        }
        
        public void BinarySerializeData(BinaryWriter bw)
        {
            return;
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            Value = BitConverter.ToSingle(RawData);
            
            // If valid connection, attempt to dehash
            if (DbConnection != null) Name = HashUtils.Lookup(DbConnection, NameHash);
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            if (ConfigData.AlwaysOutputHash || Name.Length > 0)
            {
                xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");
            }
            if (Name.Length > 0)
            {
                xw.WriteAttributeString("Name", Name);
            }
            
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