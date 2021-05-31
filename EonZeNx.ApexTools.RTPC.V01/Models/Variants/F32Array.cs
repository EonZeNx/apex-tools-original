using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class F32Array : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override string Name { get; set; } = "";
        public override string HexNameHash => ByteUtils.IntToHex(NameHash);
        public override int NameHash { get; set; }
        public override EVariantType VariantType { get; set; } = EVariantType.Float32Array;
        public override byte[] RawData { get; set; }
        public override long Offset { get; set; }
        public override uint Alignment => 4;
        public override bool Primitive => false;
        
        public float[] Value;
        

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public F32Array() { }
        public F32Array(Property prop)
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
            foreach (var f32 in Value)
            {
                s.Write(f32);
            }
        }
        
        public override void StreamDeserialize(Stream s)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            s.Seek(dataOffset, SeekOrigin.Begin);
            
            var length = s.ReadInt32();
            var values = new float[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = s.ReadSingle();
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

            string array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);
            
            var floatString = xr.ReadString();
            if (floatString.Length == 0)
            {
                Value = Array.Empty<float>();
                return;
            }
            
            var floats = floatString.Split(",");
            Value = Array.ConvertAll(floats, float.Parse);
        }

        #endregion
    }
}