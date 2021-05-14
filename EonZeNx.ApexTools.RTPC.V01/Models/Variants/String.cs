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
    public class String : IPropertyVariants
    {
        public SQLiteConnection DbConnection { get; set; }
        public string Name { get; set; } = "";

        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.String;
        public byte[] RawData { get; }
        public long Offset { get; set; }
        public uint Alignment => 0;
        public bool Primitive => false;

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

        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((uint) Offset);
            bw.Write((byte) VariantType);
        }
        
        public void BinarySerializeData(BinaryWriter bw)
        {
            // If value already exists in file, use that offset
            if (StringMap.ContainsKey(Value))
            {
                Offset = StringMap[Value];
                return;
            }
            
            ByteUtils.Align(bw, Alignment);
            Offset = bw.BaseStream.Position;
            StringMap[Value] = Offset;
            
            bw.Write(Encoding.UTF8.GetBytes(Value));
            bw.Write((byte) 0x00);
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
            List<byte> byteString = new List<byte>();
            while (true)
            {
                var thisByte = br.ReadByte();
                if (thisByte.Equals(0x00)) break;
                
                byteString.Add(thisByte);
            }
            Value = Encoding.UTF8.GetString(byteString.ToArray());
            
            // If valid connection, attempt to dehash
            if (DbConnection != null) Name = HashUtils.Lookup(DbConnection, NameHash);
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
            xw.WriteValue(Value);
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);
            Value = xr.ReadString();
        }
    }
}