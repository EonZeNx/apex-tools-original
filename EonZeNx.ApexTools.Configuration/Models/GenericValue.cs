using System;
using System.Xml;

namespace EonZeNx.ApexTools.Configuration.Models
{
    public class GenericValue<T>
    {
        public T DefaultValue { get; set; }
        public T CurrentValue { get; set; }

        
        public GenericValue(T defaultValue)
        {
            DefaultValue = defaultValue;
            CurrentValue = defaultValue;
        }
        
        public GenericValue(T defaultValue, T currentValue)
        {
            DefaultValue = defaultValue;
            CurrentValue = currentValue;
        }

        public void Load(XmlReader xr)
        {
            xr.ReadToNextSibling("Value");

            var defaultValueStr = xr.GetAttribute("DefaultValue");
            if (string.IsNullOrEmpty(defaultValueStr)) defaultValueStr = "";
            
            DefaultValue = (T) Convert.ChangeType(defaultValueStr, typeof(T));
        }
        
        public void Save(XmlWriter xw)
        {
            xw.WriteStartElement("Value");
            
            xw.WriteAttributeString("DefaultValue", $"{DefaultValue}");
            xw.WriteValue(CurrentValue);
            
            xw.WriteEndElement();
        }
    }
}