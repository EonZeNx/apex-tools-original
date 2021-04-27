using System.Xml;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IXmlSerializable
    {
        public void XmlSerialize(XmlWriter xw) {}
        public void XmlDeserialize(XmlReader xr) {}
    }
}