using System;
using System.Xml;

namespace EonZeNx.ApexTools.Configuration.Models
{
    public interface IGenericValue
    {


        public void Load(XmlReader xr);
        // {
        //     xr.ReadToNextSibling("Value");
        //
        //     var defaultValueStr = xr.GetAttribute("DefaultValue");
        //     if (string.IsNullOrEmpty(defaultValueStr)) defaultValueStr = "";
        //     
        //     DefaultValue = (T) Convert.ChangeType(defaultValueStr, typeof(T));
        // }

        public void Save(XmlWriter xw);
        // {
        //     xw.WriteStartElement("Value");
        //     
        //     xw.WriteAttributeString("DefaultValue", $"{DefaultValue}");
        //     xw.WriteValue(CurrentValue);
        //     
        //     xw.WriteEndElement();
        // }
    }
}