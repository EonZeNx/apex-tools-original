﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using A01.Interfaces.Serializable;
using A01.Utils;
using IXmlSerializable = A01.Interfaces.Serializable.IXmlSerializable;

namespace A01.Models.IRTPC.V01
{
    public class IRTPC_V01 : IXmlClassIO
    {
        /* ROOT
        * Version 01 : u8
        * Version 02 : u16
        * Object count : u16
        */

        public MetaInfo Minfo { get; set; } = new (){FileType = "IRTPC", Version = 01};
        
        protected long Offset { get; private set; }
        protected ushort ObjectCount { get; private set; }
        public byte Version01 { get; set; }
        public ushort Version02 { get; set; }
        public Container[] Containers { get; set; }


        public MetaInfo GetMetaInfo()
        {
            return Minfo;
        }

        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(Version01);
            bw.Write(Version02);
            bw.Write((ushort) Containers.Length);
            foreach (var container in Containers)
            {
                container.BinarySerialize(bw);
            }
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            Offset = BinaryReaderUtils.Position(br);
            Version01 = br.ReadByte();
            Version02 = br.ReadUInt16();
            ObjectCount = br.ReadUInt16();

            Containers = new Container[ObjectCount];
            for (int i = 0; i < ObjectCount; i++)
            {
                Containers[i] = new Container();
                Containers[i].BinaryDeserialize(br);
            }
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{Minfo.GetType().Name}");
            xw.WriteAttributeString("FileType", Minfo.FileType);
            xw.WriteAttributeString("Version", $"{Minfo.Version}");
            xw.WriteAttributeString("Extension", Minfo.Extension);
            
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("Version01", $"{Version01}");
            xw.WriteAttributeString("Version02", $"{Version02}");

            foreach (var container in Containers)
            {
                container.XmlSerialize(xw);
            }
            
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.WriteEndDocument();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            Minfo.Extension = XmlUtils.GetAttribute(xr, "Extension");
            
            xr.ReadToDescendant("IRTPC_V01");
            
            Version01 = byte.Parse(XmlUtils.GetAttribute(xr, "Version01"));
            Version02 = ushort.Parse(XmlUtils.GetAttribute(xr, "Version02"));

            var containers = new List<Container>();
            xr.ReadToDescendant("Container");
            while (xr.NodeType == XmlNodeType.Element)
            {
                if (xr.NodeType != XmlNodeType.Element || xr.Name != "Container") break;
                
                var container = new Container();
                container.XmlDeserialize(xr);
                containers.Add(container);
                
                xr.ReadToNextSibling("Container");
                if (xr.NodeType == XmlNodeType.EndElement) xr.ReadToNextSibling("Container");
            }
            xr.Close();

            Containers = containers.ToArray();
        }
    }
}