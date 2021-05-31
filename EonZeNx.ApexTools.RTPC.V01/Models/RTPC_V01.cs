using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Models;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models
{
    /// <summary>
    /// An <see cref="RTPC_V01"/> file
    /// <br/> Structure:
    /// <br/> FourCC
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Root container - <see cref="Container"/>
    /// </summary>
    public class RTPC_V01 : IXmlClassIO
    {
        public MetaInfo Minfo { get; set; } = new (){FileType = "RTPC", Version = 01};
        public SQLiteConnection DbConnection { get; set; }
        
        protected long Offset { get; private set; }
        public int Version { get; set; }
        public Container Root { get; set; }


        public MetaInfo GetMetaInfo()
        {
            return Minfo;
        }

        #region Binary Serialization

        public void StreamSerialize(Stream s)
        {
            s.Write(Encoding.UTF8.GetBytes(Minfo.FileType));
            s.Write(Version);

            var originalOffset = s.Position;
            s.Seek(Container.ContainerHeaderSize, SeekOrigin.Current);
            Root.StreamSerializeData(s);
            
            s.Seek((int) originalOffset, SeekOrigin.Begin);
            Root.StreamSerialize(s);
        }

        public void StreamDeserialize(Stream s)
        {
            Offset = s.Position;
            var fourCc = s.ReadInt32();
            Version = s.ReadInt32();
            Root = new Container(DbConnection);
            Root.StreamDeserialize(s);
        }

        #endregion

        #region XML Serialization

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

        #endregion
    }
}