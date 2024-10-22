﻿using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.IRTPC.V01.Models.Variants
{
    public class FloatArrayVariant : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override int NameHash { get; set; }
        public override string Name { get; set; }
        protected override EVariantType VariantType { get; set; }
        protected override long Offset { get; set; }
        
        protected static int NUM = 2;
        public float[] Value;

        public FloatArrayVariant() { }
        public FloatArrayVariant(Property prop)
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
            foreach (var val in Value)
            {
                bw.Write(val);
            }
        }
    
        public override void BinaryDeserialize(BinaryReader br)
        {
            Value = new float[NUM];
            for (int i = 0; i < NUM; i++)
            {
                Value[i] = br.ReadSingle();
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

            string array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);
            
            var floatString = xr.ReadString();
            var floats = floatString.Split(",");
            Value = Array.ConvertAll(floats, input => float.Parse(input));
        }

        #endregion
    }
}