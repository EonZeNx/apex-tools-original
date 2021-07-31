using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration.Models;

namespace EonZeNx.ApexTools.Configuration
{
    public class ConfigFile
    {
        public GenericSetting<bool> AutoClose = new (
            "AutoClose",
            "Auto close the program on success or failure.",
            new GenericValue<bool>(false));
        
        public GenericSetting<string> AbsolutePathToDatabase = new (
            "AbsolutePathToDatabase",
            "The absolute file path to the database for hash lookups.",
            new GenericValue<string>(Path.Combine(AppContext.BaseDirectory, "dbs", "global.db")));
        
        public GenericSetting<bool> AlwaysOutputHash = new (
            "AlwaysOutputHash",
            "When successfully finding a hash, always output that hash as well as the result.",
            new GenericValue<bool>(false));
        
        public GenericSetting<bool> PerformDehash = new (
            "PerformDehash",
            "Perform a hash lookup where possible. Useful for quick testing as hash lookups slow down conversion.",
            new GenericValue<bool>(true));
        
        public GenericSetting<int> HashCacheSize = new (
            "HashCacheSize",
            "The number of hashes to hold in memory. Speeds up frequent, common hash lookups.",
            new GenericValue<int>(250));
        
        public GenericSetting<bool> SortFiles = new (
            "SortFiles",
            "Whether or not to sort the files where possible.",
            new GenericValue<bool>(false));
        
        public GenericSetting<bool> TryFindUint32Hash = new (
            "TryFindUint32Hash",
            "Some UInt32 values are hashes. Attempt to a hash lookup on these values?",
            new GenericValue<bool>(false));
        
        public GenericSetting<bool> MergeTocToSarc = new (
            "MergeTocToSarc",
            "WIP - Merge a TOC file into a SARC file. Will result in a larger SARC and an empty TOC.",
            new GenericValue<bool>(true));
    }
    
    public static class ConfigData
    {
        #region Variables

        public static ConfigFile Data { get; set; } = new ();
        private static string ExeFilepath { get; set; }
        
        public static bool AutoClose => Data.AutoClose.Get();
        public static string AbsolutePathToDatabase => Data.AbsolutePathToDatabase.Get();
        public static bool AlwaysOutputHash => Data.AlwaysOutputHash.Get();
        public static bool PerformDehash => Data.PerformDehash.Get();
        public static int HashCacheSize => Data.HashCacheSize.Get();
        public static bool SortFiles => Data.SortFiles.Get();
        public static bool TryFindUint32Hash => Data.TryFindUint32Hash.Get();
        public static bool MergeTocToSarc => Data.MergeTocToSarc.Get();

        #endregion


        #region Save Functions

        private static void WriteSetting<T>(XmlWriter xw, GenericSetting<T> setting)
        {
            xw.WriteStartElement(setting.Name);
            
            xw.WriteStartElement(nameof(setting.Value));
            xw.WriteAttributeString(nameof(setting.Value.DefaultValue), setting.Value.DefaultValue.ToString());
            xw.WriteValue(setting.Value.CurrentValue.ToString());
            xw.WriteEndElement();
            
            xw.WriteStartElement(nameof(setting.Description));
            xw.WriteValue(setting.Description);
            xw.WriteEndElement();
            
            xw.WriteEndElement();
        }
        
        private static void WriteConfigFile(XmlWriter xw)
        {
            xw.WriteStartElement("Settings");

            WriteSetting(xw, Data.AutoClose);
            WriteSetting(xw, Data.AbsolutePathToDatabase);
            WriteSetting(xw, Data.AlwaysOutputHash);
            WriteSetting(xw, Data.PerformDehash);
            WriteSetting(xw, Data.HashCacheSize);
            WriteSetting(xw, Data.SortFiles);
            WriteSetting(xw, Data.TryFindUint32Hash);
            WriteSetting(xw, Data.MergeTocToSarc);
            
            xw.WriteEndElement();
        }

        #endregion
        
        private static void Save()
        {
            var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
            var xw = XmlWriter.Create(ExeFilepath, settings);
            WriteConfigFile(xw);
            xw.Close();
        }


        #region Load Functions

        private static T CastTo<T>(string value)
        {
            return (T) Convert.ChangeType(value, typeof(T));
        }

        private static GenericSetting<T> LoadSetting<T>(XmlReader xr, string name)
        {
            xr.ReadToFollowing(name);
            
            xr.ReadToFollowing(nameof(Data.AutoClose.Value));
            var defaultValue = xr.GetAttribute(nameof(Data.AutoClose.Value.DefaultValue));
            var currentValue = xr.ReadElementContentAsString();
            // xr.ReadEndElement();
            
            var temp = nameof(Data.AutoClose.Description);
            xr.ReadToFollowing(temp);
            xr.ReadStartElement(temp);
            var desc = xr.Value;
            
            var value = new GenericValue<T>(CastTo<T>(defaultValue), CastTo<T>(currentValue));
            return new GenericSetting<T>(name, desc, value);
        }
        
        private static void LoadConfigFile(XmlReader xr)
        {
            xr.ReadToDescendant("Settings");

            Data.AutoClose.Value.CurrentValue = LoadSetting<bool>(xr, nameof(Data.AutoClose)).Value.CurrentValue;
            Data.AbsolutePathToDatabase.Value.CurrentValue = LoadSetting<string>(xr, nameof(Data.AbsolutePathToDatabase)).Value.CurrentValue;
            Data.AlwaysOutputHash.Value.CurrentValue = LoadSetting<bool>(xr, nameof(Data.AlwaysOutputHash)).Value.CurrentValue;
            Data.PerformDehash.Value.CurrentValue = LoadSetting<bool>(xr, nameof(Data.PerformDehash)).Value.CurrentValue;
            Data.HashCacheSize.Value.CurrentValue = LoadSetting<int>(xr, nameof(Data.HashCacheSize)).Value.CurrentValue;
            Data.SortFiles.Value.CurrentValue = LoadSetting<bool>(xr, nameof(Data.SortFiles)).Value.CurrentValue;
            Data.TryFindUint32Hash.Value.CurrentValue = LoadSetting<bool>(xr, nameof(Data.TryFindUint32Hash)).Value.CurrentValue;
            Data.MergeTocToSarc.Value.CurrentValue = LoadSetting<bool>(xr, nameof(Data.MergeTocToSarc)).Value.CurrentValue;
        }

        #endregion
        
        public static void Load()
        {
            var exeDirectory = AppContext.BaseDirectory;
            ExeFilepath = Path.Combine(exeDirectory, "config.xml");
            
            if (!File.Exists(ExeFilepath)) Save();

            var xr = XmlReader.Create(ExeFilepath);
            LoadConfigFile(xr);
        }
    }
}