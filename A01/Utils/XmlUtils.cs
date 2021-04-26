using System.Xml;

namespace A01.Utils
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
    }
}