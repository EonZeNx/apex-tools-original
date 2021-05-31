using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class UIntArray : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override string Name { get; set; } = "";
        public override string HexNameHash => ByteUtils.IntToHex(NameHash);
        public override int NameHash { get; set; }
        public override EVariantType VariantType { get; set; } = EVariantType.UInteger32Array;
        public override byte[] RawData { get; set; }
        public override long Offset { get; set; }
        public override uint Alignment => 4;
        public override bool Primitive => false;
        
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


        #region Binary Serialization

        public override void StreamSerializeData(Stream s)
        {
            ByteUtils.Align(s, Alignment);
            Offset = s.Position;
            
            s.Write(Value.Length);
            foreach (var u32 in Value)
            {
                s.Write(u32);
            }
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
            
            var length = s.ReadInt32();
            var values = new uint[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = s.ReadUInt32();
            }

            Value = values;
            
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
            
            var uintString = xr.ReadString();
            if (uintString.Length == 0)
            {
                Value = Array.Empty<uint>();
                return;
            }
            
            var uints = uintString.Split(",");
            Value = Array.ConvertAll(uints, uint.Parse);
        }

        #endregion
    }
}