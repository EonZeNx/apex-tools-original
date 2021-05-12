using System;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class OID : IPropertyVariants
    {
        public SQLiteConnection DbConnection { get; set; }
        public string Name { get; set; } = "";

        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.ObjectID;
        public byte[] RawData { get; }
        public long Offset { get; set; }
        public uint Alignment => 4;
        public bool Primitive => false;
        
        public (ulong, byte) Value;

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public OID() { }
        public OID(Property prop)
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
            
            bw.Write(Value.Item1);
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
            // Thanks UnknownMiscreant
            var oid = ByteUtils.ReverseBytes(br.ReadUInt64());
            var userData = (byte)(oid & byte.MaxValue);
            
            Value = (oid, userData);
            
            // If valid connection, attempt to dehash
            if (DbConnection != null) Name = HashUtils.Lookup(DbConnection, NameHash);
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameIfValid(xw, NameHash, Name);

            var reversedOid = ByteUtils.ReverseBytes(Value.Item1);
            var stringOid = ByteUtils.UlongToHex(reversedOid);

            var stringUserData = ByteUtils.ByteToHex(Value.Item2);
            
            var full = $"{stringOid}={stringUserData}";
            xw.WriteValue(full);
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            var strValue = xr.ReadString();
            var strArray = strValue.Split("=");

            var reversedOid = ulong.Parse(strArray[0], NumberStyles.AllowHexSpecifier);
            var oid = ByteUtils.ReverseBytes(reversedOid);

            var userData = byte.Parse(strArray[1], NumberStyles.AllowHexSpecifier);
            
            Value = (oid, userData);
        }
    }
}