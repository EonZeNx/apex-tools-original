using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Models;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models
{
    public class RTPC_V01 : IXmlClassIO
    {
        /* ROOT
         * Version : s32
         */

        public MetaInfo Minfo { get; set; } = new (){FileType = "RTPC", Version = 01};
        
        protected long Offset { get; private set; }
        public int Version { get; set; }
        public Container Root { get; set; }


        public MetaInfo GetMetaInfo()
        {
            return Minfo;
        }

        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(Encoding.UTF8.GetBytes(Minfo.FileType));
            bw.Write(Version);
            Root.BinarySerialize(bw);
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            Offset = BinaryReaderUtils.Position(br);
            var fourCc = br.ReadInt32();
            Version = br.ReadInt32();
            Root = new Container();
            Root.BinaryDeserialize(br);
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{Minfo.GetType().Name}");
            xw.WriteAttributeString("FileType", Minfo.FileType);
            xw.WriteAttributeString("Version", $"{Minfo.Version}");
            xw.WriteAttributeString("Extension", Minfo.Extension);
            
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("Version", $"{Version}");

            Root.XmlSerialize(xw);
            
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.WriteEndDocument();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            Minfo.Extension = XmlUtils.GetAttribute(xr, "Extension");
            
            xr.ReadToDescendant("RTPC_V01");
            Version = byte.Parse(XmlUtils.GetAttribute(xr, "Version"));

            Root = new Container();
            xr.ReadToDescendant($"{Root.GetType().Name}");
            Root.XmlDeserialize(xr);
            
            xr.Close();
        }
    }
}