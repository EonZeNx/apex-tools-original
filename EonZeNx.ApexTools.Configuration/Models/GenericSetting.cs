using System.Xml;

namespace EonZeNx.ApexTools.Configuration.Models
{
    public class GenericSetting<T> : IGenericSetting<T>
    {
       public string Name { get; }
       public string Description { get; }
       public GenericValue<T> Value { get; }
       
       
       public GenericSetting(string name, string description, GenericValue<T> value)
       {
           Name = name;
           Description = description;
           Value = value;
       }

       public void Load(XmlReader xr)
       {
           xr.ReadToDescendant("Value");
           Value.Load(xr);
       }

       public T Get() { return Value.CurrentValue; }

       public void Save(XmlWriter xw)
       {
           xw.WriteStartElement(GetType().Name);
           
           xw.WriteStartElement("Desc");
           xw.WriteValue(Description);
           xw.WriteEndElement();
           
           Value.Save(xw);
           
           xw.WriteEndElement();
       }
    }
}