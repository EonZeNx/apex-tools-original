using System.Xml;
using EonZeNx.ApexTools.Configuration;

namespace EonZeNx.ApexTools.Core.Utils
{
    public static class XmlUtils
    {
        public static string GetAttribute(XmlReader xr, string attribute)
        {
            if (!xr.HasAttributes) throw new XmlException("Missing attributes");
            
            var attributeValue = xr.GetAttribute(attribute);
            if (attributeValue == null) throw new XmlException($"Missing attribute '{attribute}'");
            
            return attributeValue;
        }

        public static void WriteNameIfValid(XmlWriter xw, int nameHash, string name = "")
        {
            if (ConfigData.AlwaysOutputHash || name.Length == 0)
            {
                xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(nameHash)}");
            }
            if (name.Length > 0)
            {
                xw.WriteAttributeString("Name", name);
            }
        }
    }
}