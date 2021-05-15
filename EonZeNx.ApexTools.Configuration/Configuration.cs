using System.IO;
using System.Xml.Serialization;

namespace EonZeNx.ApexTools.Configuration
{
    public class ConfigFile
    {
        public bool AutoClose { get; set; } = false;
        public string AbsolutePathToDatabase { get; set; } = Path.GetFullPath(@"dbs\global.db");
        public bool AlwaysOutputHash { get; set; } = false;
        public bool PerformDehash { get; set; } = true;
        public int HashCacheSize { get; set; } = 500;
        public bool SortFiles { get; set; } = false;
    }
    
    public static class ConfigData
    {
        private static ConfigFile Data { get; set; } = new ();
        
        public static bool AutoClose => Data.AutoClose;
        public static string AbsolutePathToDatabase => Data.AbsolutePathToDatabase;
        public static bool AlwaysOutputHash => Data.AlwaysOutputHash;
        public static bool PerformDehash => Data.PerformDehash;
        public static int HashCacheSize => Data.HashCacheSize;
        public static bool SortFiles => Data.SortFiles;

        public static void Load()
        {
            if (!File.Exists(@"config.xml")) Save();
            
            var xs = new XmlSerializer(typeof(ConfigFile));
            using (var fs = new FileStream(@".\config.xml", FileMode.Open))
            {
                Data = (ConfigFile) xs.Deserialize(fs);
            }
        }

        private static void Save()
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            
            var xs = new XmlSerializer(typeof(ConfigFile));
            using (var fs = new FileStream(@".\config.xml", FileMode.Create))
            {
                xs.Serialize(fs, Data, ns);
            }
        }
    }
}