using System;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class OID : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override string Name { get; set; } = "";
        public override string HexNameHash => ByteUtils.IntToHex(NameHash);
        public override int NameHash { get; set; }
        public override EVariantType VariantType { get; set; }  = EVariantType.ObjectID;
        public override byte[] RawData { get; set; }
        public override long Offset { get; set; }
        public override uint Alignment => 4;
        public override bool Primitive => false;
        
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


        #region Binary Serialization

        public override void BinarySerializeData(BinaryWriter bw)
        {
            ByteUtils.Align(bw, Alignment);
            Offset = bw.BaseStream.Position;
            
            bw.Write(Value.Item1);
        }
        
        public override void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((uint) Offset);
            bw.Write((byte) VariantType);
        }
        
        public override void BinaryDeserialize(BinaryReader br)
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

        #endregion

        #region XML Serialization

        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);

            var reversedOid = ByteUtils.ReverseBytes(Value.Item1);
            var stringOid = ByteUtils.UlongToHex(reversedOid);

            var stringUserData = ByteUtils.ByteToHex(Value.Item2);
            
            var full = $"{stringOid}={stringUserData}";
            xw.WriteValue(full);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);
            
            var strValue = xr.ReadString();
            var strArray = strValue.Split("=");

            var reversedOid = ulong.Parse(strArray[0], NumberStyles.AllowHexSpecifier);
            var oid = ByteUtils.ReverseBytes(reversedOid);

            var userData = byte.Parse(strArray[1], NumberStyles.AllowHexSpecifier);
            
            Value = (oid, userData);
        }

        #endregion
    }
}