using System.Xml;

namespace A01.Interfaces.Serializable
{
    public interface IXmlSerializable
    {
        public void XmlSerialize(XmlWriter xw) {}
        public void XmlDeserialize(XmlReader xr) {}
    }
}