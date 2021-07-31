using System.Xml;

namespace EonZeNx.ApexTools.Configuration.Models
{
    public interface IGenericSetting<out T>
    {
       string Name { get; }
       string Description { get; }

       T Get();
    }
}