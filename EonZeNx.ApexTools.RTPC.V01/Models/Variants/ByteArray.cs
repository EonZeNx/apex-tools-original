using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class ByteArray : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override string Name { get; set; } = "";
        public override string HexNameHash => ByteUtils.IntToHex(NameHash);
        public override int NameHash { get; set; }
        public override EVariantType VariantType { get; set; } = EVariantType.ByteArray;
        public override byte[] RawData { get; set; }
        public override long Offset { get; set; }
        public override uint Alignment => 4;
        public override bool Primitive => false;
        
        public byte[] Value;

        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public ByteArray() { }
        public ByteArray(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }

        
        #region Binary Serialization

        public override void StreamSerialize(Stream s)
        {
            s.Write(NameHash);
            s.Write((uint) Offset);
            s.Write((byte) VariantType);
        }
        
        public override void StreamSerializeData(Stream s)
        {
            ByteUtils.Align(s, Alignment);
            Offset = s.Position;
            
            s.Write(Value.Length);
            foreach (var val in Value)
            {
                s.Write(val);
            }
        }

        public override void StreamDeserialize(Stream s)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            s.Seek(dataOffset, SeekOrigin.Begin);
            
            var length = s.ReadInt32();
            if (length != 0)
            {
                Value = s.ReadBytes(length);
            }
            
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

            var array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);
            
            var floatString = xr.ReadString();
            var floats = floatString.Split(",");
            Value = Array.ConvertAll(floats, input => byte.Parse(input));
        }

        #endregion
    }
}