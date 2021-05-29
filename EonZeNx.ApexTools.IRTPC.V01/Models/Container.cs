using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.IRTPC.V01.Models.Variants;
using IXmlSerializable = EonZeNx.ApexTools.Core.Interfaces.Serializable.IXmlSerializable;
using String = EonZeNx.ApexTools.IRTPC.V01.Models.Variants.String;
using UInt32 = EonZeNx.ApexTools.IRTPC.V01.Models.Variants.UInt32;

namespace EonZeNx.ApexTools.IRTPC.V01.Models
{
    public class Container : IBinarySerializable, IXmlSerializable
    {
        /* CONTAINER
         * Name hash : s32
         * Version 01 : u8
         * Version 02 : u16
         * Object count : u16
         * NOTE: Containers only contain properties
         */
        
        public SQLiteConnection DbConnection { get; set; }

        public int NameHash { get; set; }
        public string Name { get; set; }
        public string HexNameHash => ByteUtils.IntToHex(NameHash);
        private ushort ObjectCount { get; set; }
        public byte Version01 { get; set; }
        public ushort Version02 { get; set; }
        private long Offset { get; set; }
        public PropertyVariants[] Properties { get; set; }


        
        public Container(SQLiteConnection con = null)
        {
            DbConnection = con;
        }

        #region Helpers

        private void SortProperties()
        {
            if (ConfigData.SortFiles)
            {
                Array.Sort(Properties, new PropertyComparer());
            }
        }

        #endregion

        #region Binary Serialization

        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write(Version01);
            bw.Write(Version02);
            bw.Write(ObjectCount);
            foreach (var property in Properties)
            {
                property.BinarySerialize(bw);
            }
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            Offset = BinaryReaderUtils.Position(br);
            NameHash = br.ReadInt32();
            Version01 = br.ReadByte();
            Version02 = br.ReadUInt16();
            ObjectCount = br.ReadUInt16();
            
            // If valid connection, attempt to dehash
            if (DbConnection != null) Name = HashUtils.Lookup(DbConnection, NameHash);

            Properties = new PropertyVariants[ObjectCount];
            for (int i = 0; i < ObjectCount; i++)
            {
                var prop = new Property(br, DbConnection);
                Properties[i] = prop.Type switch
                {
                    EVariantType.UInteger32 => new UInt32(prop),
                    EVariantType.Float32 => new F32(prop),
                    EVariantType.String => new String(prop),
                    EVariantType.Vec2 => new Vec2(prop),
                    EVariantType.Vec3 => new Vec3(prop),
                    EVariantType.Vec4 => new Vec4(prop),
                    EVariantType.Mat3X4 => new Mat3X4(prop),
                    EVariantType.Event => new Event(prop),
                    _ => throw new InvalidEnumArgumentException($"Property type was not a valid variant - {prop.Type}")
                };
                Properties[i].BinaryDeserialize(br);
            }
            
            SortProperties();
        }

        #endregion

        #region XML Serialization

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
            xw.WriteAttributeString("Version01", $"{Version01}");
            xw.WriteAttributeString("Version02", $"{Version02}");
            
            foreach (var property in Properties)
            {
                property.XmlSerialize(xw);
            }
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);
            Version01 = byte.Parse(XmlUtils.GetAttribute(xr, "Version01"));
            Version02 = ushort.Parse(XmlUtils.GetAttribute(xr, "Version02"));

            var properties = new List<PropertyVariants>();
            xr.Read();

            while (xr.Read())
            {
                var tag = xr.Name;
                var nodeType = xr.NodeType;
                
                if (tag == "Container" && nodeType == XmlNodeType.EndElement) break;
                if (nodeType != XmlNodeType.Element) continue;
                
                if (!xr.HasAttributes) throw new XmlException("Property missing attributes");
                
                var propertyType = tag;
                PropertyVariants property = propertyType switch
                {
                    "UInt32" => new UInt32(),
                    "F32" => new F32(),
                    "String" => new String(),
                    "Vec2" => new Vec2(),
                    "Vec3" => new Vec3(),
                    "Vec4" => new Vec4(),
                    "Mat3X4" => new Mat3X4(),
                    "Event" => new Event(),
                    _ => new UInt32()
                };

                property.XmlDeserialize(xr);
                properties.Add(property);
            }

            Properties = properties.ToArray();
            ObjectCount = (ushort) Properties.Length;
            
            SortProperties();
        }

        #endregion
    }
}













