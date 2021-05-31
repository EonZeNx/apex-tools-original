using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class F32 : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override string Name { get; set; } = "";
        public override string HexNameHash => ByteUtils.IntToHex(NameHash);
        public override int NameHash { get; set; }
        public override EVariantType VariantType { get; set; } = EVariantType.Float32;
        public override byte[] RawData { get; set; }
        public override long Offset { get; set; }
        public override uint Alignment => 0;
        public override bool Primitive => true;
        
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


        #region Binary Serialization

        public override void StreamSerialize(Stream s)
        {
            s.Write(NameHash);
            s.Write(Value);
            s.Write((byte) VariantType);
        }
        
        public override void StreamSerializeData(Stream s)
        {
            return;
        }
        
        public override void StreamDeserialize(Stream s)
        {
            Value = BitConverter.ToSingle(RawData);
            
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
            
            xw.WriteValue(Value);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);
            Value = float.Parse(xr.ReadString());
        }

        #endregion
    }
}