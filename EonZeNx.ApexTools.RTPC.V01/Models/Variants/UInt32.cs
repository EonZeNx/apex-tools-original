using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class UInt32 : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override string Name { get; set; } = "";
        public override string HexNameHash => ByteUtils.IntToHex(NameHash);
        public override int NameHash { get; set; }
        public override EVariantType VariantType { get; set; } = EVariantType.UInteger32;
        public override byte[] RawData { get; set; }
        public override long Offset { get; set; }
        public override uint Alignment => 0;
        public override bool Primitive => true;

        public static int[] ValuesToSkipLookup => new[] {0, 1, 2, 3};
        
        public uint Value;
        public string LookupValue;

        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public UInt32() { }
        public UInt32(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
            DbConnection = prop.DbConnection;
        }


        #region Binary Serialization

        public override void BinarySerializeData(BinaryWriter bw)
        {
            return;
        }
        
        public override void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write(Value);
            bw.Write((byte) VariantType);
        }

        public override void BinaryDeserialize(BinaryReader br)
        {
            Value = BitConverter.ToUInt32(RawData);
            
            // If valid connection, attempt lookup
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
            var valueHash = XmlUtils.GetAttribute(xr, "Hash");
            var rawValue = xr.ReadElementContentAsString();
            
            if (string.IsNullOrEmpty(valueHash))
            {
                Value = uint.Parse(rawValue);
            }
            else
            {
                // Do this instead of error catching and throwing, it's much faster
                LookupValue = rawValue;
                Value = (uint) HashUtils.HashJenkinsL3(LookupValue);
            }
        }

        #endregion
    }
}