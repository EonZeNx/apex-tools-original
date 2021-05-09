using System.IO;
using System.Xml.Serialization;

namespace EonZeNx.ApexTools.Configuration
{
    public class ConfigFile
    {
        public bool AutoClose { get; set; } = false;
    }
    
    public class ConfigData
    {
        public ConfigFile Data { get; set; }
        public bool AutoClose => Data.AutoClose;


        public ConfigData()
        {
            Data = new ConfigFile();
        }
        
        public void Load()
        {
            if (!File.Exists(@"config.xml")) Save();
            
            var xs = new XmlSerializer(typeof(ConfigFile));
            using (var fs = new FileStream(@".\config.xml", FileMode.Open))
            {
                Data = (ConfigFile) xs.Deserialize(fs);
            }
        }

        public void Save()
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            
            var xs = new XmlSerializer(typeof(ConfigFile));
            using (var fs = new FileStream(@".\config.xml", FileMode.Create))
            {
                xs.Serialize(fs, Data, ns);
            }
        }
    }
}