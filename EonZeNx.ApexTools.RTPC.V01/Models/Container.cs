﻿using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.RTPC.V01.Models.Variants;

namespace EonZeNx.ApexTools.RTPC.V01.Models
{
    public class Container : IBinarySerializable, IXmlSerializable
    {
        /* CONTAINER
         * Name hash : s32
         * Offset : u32
         * Property count : u16
         * Container count : u16
         * PROPERTIES[]
         * CONTAINERS[]
         * NOTE: Can have both properties & other containers
         */
        
        public int NameHash { get; set; }
        public uint Offset { get; set; }
        public ushort PropertyCount { get; set; }
        public ushort ContainerCount { get; set; }
        public IPropertyVariants[] Properties { get; set; }
        public Container[] Containers { get; set; }
        
        public Property[] PropertyHeaders { get; set; }
        public long ContainerHeaderOffset { get; set; }


        #region Binary Load Helpers

        private void BinaryLoadProperties(BinaryReader br)
        {
            br.BaseStream.Position = Offset;
            PropertyHeaders = new Property[PropertyCount];
            for (int i = 0; i < PropertyCount; i++)
            {
                PropertyHeaders[i] = new Property(br);
            }

            ContainerHeaderOffset = br.BaseStream.Position;

            Properties = new IPropertyVariants[PropertyCount];
            for (int i = 0; i < PropertyHeaders.Length; i++)
            {
                var prop = PropertyHeaders[i];
                Properties[i] = prop.Type switch
                {
                    EVariantType.Unassigned => throw new InvalidEnumArgumentException("Property type was not a valid variant (Unassigned)."),
                    EVariantType.UInteger32 => new UInt32(prop),
                    EVariantType.Float32 => new F32(prop),
                    EVariantType.String => new String(prop),
                    EVariantType.Vec2 => new Vec2(prop),
                    EVariantType.Vec3 => new Vec3(prop),
                    EVariantType.Vec4 => new Vec4(prop),
                    EVariantType.Mat3X3 => new Mat3X3(prop),
                    EVariantType.Mat4X4 => new Mat4X4(prop),
                    EVariantType.UInteger32Array => new UIntArray(prop),
                    EVariantType.Float32Array => new F32Array(prop),
                    EVariantType.ByteArray => new ByteArray(prop),
                    EVariantType.Deprecated => throw new InvalidEnumArgumentException("Property type was not a valid variant (Deprecated)."),
                    EVariantType.ObjectID => new OID(prop),
                    EVariantType.Event => new Event(prop),
                    EVariantType.Total => throw new InvalidEnumArgumentException("Property type was not a valid variant (Total)."),
                    _ => throw new InvalidEnumArgumentException("Property type was not a valid variant (Unknown type).")
                };

                Properties[i].BinaryDeserialize(br);
            }
        }

        private void BinaryLoadContainers(BinaryReader br)
        {
            br.BaseStream.Seek(ContainerHeaderOffset, SeekOrigin.Begin);
            ByteUtils.Align(br, 4);
            Containers = new Container[ContainerCount];
            for (int i = 0; i < ContainerCount; i++)
            {
                Containers[i] = new Container();
                Containers[i].BinaryDeserialize(br);
            }
        }

        #endregion

        #region Xml Load Helpers

        private void XmlLoadProperties(XmlReader xr)
        {
            var properties = new List<IPropertyVariants>();
            xr.Read();

            while (xr.Read())
            {
                var tag = xr.Name;
                var nodeType = xr.NodeType;
                
                if (tag == "Container") break;
                if (nodeType != XmlNodeType.Element) continue;
                
                if (!xr.HasAttributes) throw new XmlException("Property missing attributes");
                
                var propertyType = xr.Name;
                IPropertyVariants property = propertyType switch
                {
                    "Unassigned" => throw new InvalidEnumArgumentException("Property type was not a valid variant (Unassigned)."),
                    "UInt32" => new UInt32(),
                    "F32" => new F32(),
                    "String" => new String(),
                    "Vec2" => new Vec2(),
                    "Vec3" => new Vec3(),
                    "Vec4" => new Vec4(),
                    "Mat3X3" => new Mat3X3(),
                    "Mat4X4" => new Mat4X4(),
                    "UIntArray" => new UIntArray(),
                    "F32Array" => new F32Array(),
                    "ByteArray" => new ByteArray(),
                    "Deprecated" => throw new InvalidEnumArgumentException("Property type was not a valid variant (Deprecated)."),
                    "OID" => new OID(),
                    "Event" => new Event(),
                    "Total" => throw new InvalidEnumArgumentException("Property type was not a valid variant (Total)."),
                    _ => throw new InvalidEnumArgumentException("Property type was not a valid variant (Unknown type).")
                };

                property.XmlDeserialize(xr);
                properties.Add(property);
            }

            Properties = properties.ToArray();
            PropertyCount = (ushort) Properties.Length;
        }
        
        private void XmlLoadContainers(XmlReader xr)
        {
            var containers = new List<Container>();

            do
            {
                var tag = xr.Name;
                var nodeType = xr.NodeType;
                
                if (tag == "Container" && nodeType == XmlNodeType.EndElement) break;
                if (nodeType != XmlNodeType.Element) continue;
                
                if (!xr.HasAttributes) throw new XmlException("Property missing attributes");

                var container = new Container();

                container.XmlDeserialize(xr);
                containers.Add(container);
            } 
            while (xr.Read());

            Containers = containers.ToArray();
            ContainerCount = (ushort) Containers.Length;
        }

        #endregion

        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write(bw.BaseStream.Position);
            bw.Write(PropertyCount);
            bw.Write(ContainerCount);

            foreach (var property in Properties)
            {
                property.BinarySerialize(bw);
            }
            
            foreach (var container in Containers)
            {
                container.BinarySerialize(bw);
            }
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            NameHash = br.ReadInt32();
            Offset = br.ReadUInt32();
            PropertyCount = br.ReadUInt16();
            ContainerCount = br.ReadUInt16();

            var originalPosition = br.BaseStream.Position;
            BinaryLoadProperties(br);
            BinaryLoadContainers(br);
            br.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");
            
            foreach (var property in Properties)
            {
                property.XmlSerialize(xw);
            }
            foreach (var container in Containers)
            {
                container.XmlSerialize(xw);
            }
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);

            // TODO: Check if the container has properties
            XmlLoadProperties(xr);
            XmlLoadContainers(xr);
        }
    }
}