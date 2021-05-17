using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.IRTPC.V01.Models.Variants
{
    public class UInt32 : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override int NameHash { get; set; }
        public override string Name { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.UInteger32;
        protected override long Offset { get; set; }
        
        public uint Value;
        public string LookupValue;

        public UInt32() { }
        public UInt32(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            DbConnection = prop.DbConnection;
        }

        #region Binary Serialization

        public override void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            bw.Write(Value);
        }
        
        public override void BinaryDeserialize(BinaryReader br)
        {
            Value = br.ReadUInt32();
            
            // If valid connection, attempt to dehash
            if (DbConnection == null) return;
            
            Name = HashUtils.Lookup(DbConnection, NameHash);
            var valueInt = (int) Value;
            
            if (ConfigData.TryFindUint32Hash && (valueInt > 10 || valueInt < -10)) LookupValue = HashUtils.Lookup(DbConnection, valueInt);
        }

        #endregion

        #region XML Serialization

        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
            if (!string.IsNullOrEmpty(LookupValue))
            {
                xw.WriteAttributeString("Hash", $"{Value}");
                xw.WriteValue(LookupValue);
            }
            else
            {
                xw.WriteValue(Value);
            }
            
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);
            var rawValue = xr.ReadString();
            
            if (XmlUtils.GetAttribute(xr, "Hash").Length > 0)
            {
                LookupValue = rawValue;
                Value = (uint) HashUtils.HashJenkinsL3(LookupValue);
            }
            else
            {
                Value = uint.Parse(rawValue);
            }
        }

        #endregion
    }
}