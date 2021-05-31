using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class String : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override string Name { get; set; } = "";
        public override string HexNameHash => ByteUtils.IntToHex(NameHash);
        public override int NameHash { get; set; }
        public override EVariantType VariantType { get; set; } = EVariantType.String;
        public override byte[] RawData { get; set; }
        public override long Offset { get; set; }
        public override uint Alignment => 0;
        public override bool Primitive => false;

        public static readonly Dictionary<string, long> StringMap = new();

        public string Value;

        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public String() { }
        public String(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
            DbConnection = prop.DbConnection;
        }


        #region Binary Serialization

        public override void StreamSerializeData(Stream s)
        {
            // If value already exists in file, use that offset
            if (StringMap.ContainsKey(Value))
            {
                Offset = StringMap[Value];
                return;
            }
            
            ByteUtils.Align(s, Alignment);
            Offset = s.Position;
            StringMap[Value] = Offset;
            
            s.Write(Encoding.UTF8.GetBytes(Value));
            s.Write((byte) 0x00);
        }
        
        public override void StreamSerialize(Stream s)
        {
            s.Write(NameHash);
            s.Write((uint) Offset);
            s.Write((byte) VariantType);
        }
        
        public override void StreamDeserialize(Stream s)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            s.Seek(dataOffset, SeekOrigin.Begin);
            
            List<byte> byteString = new List<byte>();
            while (true)
            {
                var thisByte = s.ReadUByte();
                if (thisByte.Equals(0x00)) break;
                
                byteString.Add(thisByte);
            }
            Value = Encoding.UTF8.GetString(byteString.ToArray());
            
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
            Value = xr.ReadString();
        }

        #endregion
    }
}