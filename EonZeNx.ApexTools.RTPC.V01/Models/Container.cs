using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.RTPC.V01.Models.Variants;
using String = EonZeNx.ApexTools.RTPC.V01.Models.Variants.String;
using UInt32 = EonZeNx.ApexTools.RTPC.V01.Models.Variants.UInt32;

namespace EonZeNx.ApexTools.RTPC.V01.Models
{
    /// <summary>
    /// A <see cref="Container"/> holding <see cref="Properties"/> and other <see cref="Containers"/>
    /// <br/> Structure:
    /// <br/> Name hash - <see cref="int"/>
    /// <br/> Offset - <see cref="uint"/>
    /// <br/> Property count - <see cref="ushort"/>
    /// <br/> Container count - <see cref="ushort"/>
    /// <br/> Properties[]
    /// <br/> Containers[]
    /// </summary>
    public class Container : IBinarySerializable, IXmlSerializable, IDeferredSerializable
    {
        public SQLiteConnection DbConnection { get; set; }
        
        public int NameHash { get; set; }
        public string Name { get; set; }
        public string HexNameHash => ByteUtils.IntToHex(NameHash);
        public long Offset { get; set; }
        public ushort PropertyCount { get; set; }
        public ushort ContainerCount { get; set; }
        public PropertyVariants[] Properties { get; set; }
        public Container[] Containers { get; set; }

        public const int ContainerHeaderSize = 4 + 4 + 2 + 2;
        public const int PropertyHeaderSize = 4 + 4 + 1;

        public long DataPos;
        public long ContainerHeaderStart;
        
        public Property[] PropertyHeaders { get; set; }
        public long ContainerHeaderOffset { get; set; }


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
        
        private void SortContainers()
        {
            if (ConfigData.SortFiles)
            {
                Array.Sort(Containers, new ContainerComparer());
            }
        }

        #endregion
        
        #region Binary Load Helpers

        private void BinaryLoadProperties(BinaryReader br)
        {
            br.BaseStream.Position = Offset;
            PropertyHeaders = new Property[PropertyCount];
            for (int i = 0; i < PropertyCount; i++)
            {
                PropertyHeaders[i] = new Property(br, DbConnection);
            }

            ContainerHeaderOffset = br.BaseStream.Position;

            Properties = new PropertyVariants[PropertyCount];
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

            SortProperties();
        }

        private void BinaryLoadContainers(BinaryReader br)
        {
            br.BaseStream.Seek(ContainerHeaderOffset, SeekOrigin.Begin);
            ByteUtils.Align(br, 4);
            Containers = new Container[ContainerCount];
            for (int i = 0; i < ContainerCount; i++)
            {
                Containers[i] = new Container(DbConnection);
                Containers[i].BinaryDeserialize(br);
            }

            SortContainers();
        }

        #endregion

        #region Xml Load Helpers

        private void XmlLoadProperties(XmlReader xr)
        {
            var properties = new List<PropertyVariants>();
            xr.Read();

            while (xr.Read())
            {
                var tag = xr.Name;
                var nodeType = xr.NodeType;
                
                if (tag == "Container") break;
                if (nodeType != XmlNodeType.Element) continue;
                
                if (!xr.HasAttributes) throw new XmlException("Property missing attributes");
                
                var propertyType = xr.Name;
                PropertyVariants property = propertyType switch
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

            SortProperties();
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

            SortContainers();
        }

        #endregion


        #region BinarySerializable

        #region BinarySerializable Helpers

        private void BinarySerializeProperties(BinaryWriter bw)
        {
            bw.Seek((int) DataPos, SeekOrigin.Begin);
            
            foreach (var property in Properties)
            {
                property.BinarySerializeData(bw);
            }

            ByteUtils.Align(bw, 4);
            DataPos = bw.BaseStream.Position;

            bw.Seek((int) Offset, SeekOrigin.Begin);
            
            foreach (var property in Properties)
            {
                property.BinarySerialize(bw);
            }
            
            // TODO: On final property in file, fix this to not align
            ByteUtils.Align(bw, 4);
        }
        
        private void BinarySerializeContainers(BinaryWriter bw)
        {
            bw.Seek((int) DataPos, SeekOrigin.Begin);
            
            foreach (var container in Containers)
            {
                container.BinarySerializeData(bw);
            }

            DataPos = bw.BaseStream.Position;
            bw.Seek((int) ContainerHeaderStart, SeekOrigin.Begin);
            ByteUtils.Align(bw, 4);
            
            foreach (var container in Containers)
            {
                container.BinarySerialize(bw);
            }
        }

        #endregion

        public void BinarySerializeData(BinaryWriter bw)
        {
            
            Offset = bw.BaseStream.Position;
            ContainerHeaderStart = ByteUtils.Align(Offset + PropertyCount * PropertyHeaderSize, 4);
            DataPos = ContainerHeaderStart + ContainerCount * ContainerHeaderSize;

            if (Properties.Length > 0)
            {
                BinarySerializeProperties(bw);
            }

            if (Containers.Length > 0)
            {
                BinarySerializeContainers(bw);
            }

            bw.Seek((int) DataPos, SeekOrigin.Begin);
        }

        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((uint) Offset);
            bw.Write(PropertyCount);
            bw.Write(ContainerCount);
        }


        public void BinaryDeserialize(BinaryReader br)
        {
            // Read variables
            NameHash = br.ReadInt32();
            Offset = br.ReadUInt32();
            PropertyCount = br.ReadUInt16();
            ContainerCount = br.ReadUInt16();
            
            // If valid connection, attempt to dehash
            if (DbConnection != null) Name = HashUtils.Lookup(DbConnection, NameHash);

            // Read properties and sub-containers
            var originalPosition = br.BaseStream.Position;
            BinaryLoadProperties(br);
            BinaryLoadContainers(br);
            br.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
        }

        #endregion

        #region XmlSerializable

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            if (ConfigData.AlwaysOutputHash || string.IsNullOrEmpty(Name))
            {
                xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");
            }
            if (!string.IsNullOrEmpty(Name))
            {
                xw.WriteAttributeString("Name", Name);
            }
            
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
            NameHash = XmlUtils.ReadNameIfValid(xr);

            XmlLoadProperties(xr);
            XmlLoadContainers(xr);
        }

        #endregion
    }
}