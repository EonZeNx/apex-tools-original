using System.IO;
using System.Xml;
using A01.Models;

namespace A01.Interfaces.Serializable
{
    public interface IXmlClassIO : IBinarySerializable, IXmlSerializable
    {
        MetaInfo GetMetaInfo();
    }
}