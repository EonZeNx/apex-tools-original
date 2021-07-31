using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Refresh;

namespace EonZeNx.ApexTools.Core.Utils
{
    public static class XmlUtils
    {
        /// <summary>
        /// Attempts to get an attribute from an XML reader
        /// </summary>
        /// <param name="xr">XML reader</param>
        /// <param name="attribute">Attribute name</param>
        /// <returns>String if found, null if not</returns>
        /// <exception cref="XmlException">Throws exception if the element has no attributes</exception>
        public static string GetAttribute(XmlReader xr, string attribute)
        {
            if (!xr.HasAttributes) throw new XmlException("Missing attributes");

            return xr.GetAttribute(attribute) ?? "";
        }

        public static void WriteNameOrNameHash(XmlWriter xw, int nameHash, string name = "")
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

        public static int ReadNameIfValid(XmlReader xr)
        {
            var name = GetAttribute(xr, "Name");
            return name == ""
                ? ByteUtils.HexToInt(GetAttribute(xr, "NameHash"))
                : HashUtils.HashJenkinsL3(Encoding.UTF8.GetBytes(name));
        }

        public static void WriteHistory(XmlWriter xw, HistoryInstance[] history, string extension = null)
        {
            xw.WriteStartElement("History");
            if (!string.IsNullOrEmpty(extension)) xw.WriteAttributeString("Extension", extension);

            foreach (var hInstance in history)
            {
                xw.WriteStartElement("HistoryInstance");
                
                xw.WriteAttributeString("Version", hInstance.Version.ToString());
                xw.WriteValue($"{hInstance.FourCc}");
                
                xw.WriteEndElement();
            }
            
            xw.WriteEndElement();
        }
    }
}