using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.IRTPC.V01.Models.Variants;
using IXmlSerializable = EonZeNx.ApexTools.Core.Interfaces.Serializable.IXmlSerializable;

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

        public int NameHash { get; set; }
        private ushort ObjectCount { get; set; }
        public byte Version01 { get; set; }
        public ushort Version02 { get; set; }
        private long Offset { get; set; }
        public PropertyVariants[] Properties { get; set; }


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

            Properties = new PropertyVariants[ObjectCount];
            for (int i = 0; i < ObjectCount; i++)
            {
                var prop = new Property(br);
                switch (prop.Type)
                {
                    case EVariantType.UInteger32:
                        Properties[i] = new UInt32(prop); break;
                    case EVariantType.Float32:
                        Properties[i] = new F32(prop); break;
                    case EVariantType.String:
                        Properties[i] = new String(prop); break;
                    case EVariantType.Vec2:
                        Properties[i] = new Vec2(prop); break;
                    case EVariantType.Vec3:
                        Properties[i] = new Vec3(prop); break;
                    case EVariantType.Vec4:
                        Properties[i] = new Vec4(prop); break;
                    case EVariantType.Mat3X4:
                        Properties[i] = new Mat3X4(prop); break;
                    case EVariantType.Event:
                        Properties[i] = new Event(prop); break;
                    default:
                        throw new InvalidEnumArgumentException($"Property type was not a valid variant - {prop.Type}");
                }
                Properties[i].BinaryDeserialize(br);
            }
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");
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
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            Version01 = byte.Parse(XmlUtils.GetAttribute(xr, "Version01"));
            Version02 = ushort.Parse(XmlUtils.GetAttribute(xr, "Version02"));

            var properties = new List<PropertyVariants>();
            xr.Read();
            string tag;
            XmlNodeType nodeType;
            
            while (xr.Read())
            {
                tag = xr.Name;
                nodeType = xr.NodeType;
                
                if (tag == "Container" && nodeType == XmlNodeType.EndElement) break;
                if (nodeType != XmlNodeType.Element) continue;
                
                if (!xr.HasAttributes) throw new XmlException("Property missing attributes");
                
                var propertyType = xr.Name;
                PropertyVariants property;
                switch (propertyType)
                {
                    case "UInt32":
                        property = new UInt32(); break;
                    case "F32":
                        property = new F32(); break;
                    case "String":
                        property = new String(); break;
                    case "Vec2":
                        property = new Vec2(); break;
                    case "Vec3":
                        property = new Vec3(); break;
                    case "Vec4":
                        property = new Vec4(); break;
                    case "Mat3X4":
                        property = new Mat3X4(); break;
                    case "Event":
                        property = new Event(); break;
                    default:
                        property = new UInt32(); break;
                }

                property.XmlDeserialize(xr);
                properties.Add(property);
            }

            Properties = properties.ToArray();
            ObjectCount = (ushort) Properties.Length;
        }
    }
}













