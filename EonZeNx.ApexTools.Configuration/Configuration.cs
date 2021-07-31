using System;
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
        public bool TryFindUint32Hash { get; set; } = false;
        public bool MergeTocToSarc { get; set; } = true;
    }
    
    public static class ConfigData
    {
        private static ConfigFile Data { get; set; } = new ();
        private static string ExeFilepath { get; set; }
        
        public static bool AutoClose => Data.AutoClose;
        public static string AbsolutePathToDatabase => Data.AbsolutePathToDatabase;
        public static bool AlwaysOutputHash => Data.AlwaysOutputHash;
        public static bool PerformDehash => Data.PerformDehash;
        public static int HashCacheSize => Data.HashCacheSize;
        public static bool SortFiles => Data.SortFiles;
        public static bool TryFindUint32Hash => Data.TryFindUint32Hash;
        public static bool MergeTocToSarc => Data.MergeTocToSarc;

        public static void Load()
        {
            var exeDirectory = AppContext.BaseDirectory;
            ExeFilepath = Path.Combine(exeDirectory, "config.xml");
            
            if (!File.Exists(ExeFilepath)) Save();
            
            var xs = new XmlSerializer(typeof(ConfigFile));
            using var fs = new FileStream(ExeFilepath, FileMode.Open);
            Data = (ConfigFile) xs.Deserialize(fs);
        }

        private static void Save()
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            
            var xs = new XmlSerializer(typeof(ConfigFile));
            using var fs = new FileStream(ExeFilepath, FileMode.Create);
            xs.Serialize(fs, Data, ns);
        }
    }
}