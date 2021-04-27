using System;
using Microsoft.Extensions.Configuration;

namespace EonZeNx.ApexTools.Configuration
{
    public static class ConfigExtensions
    {
        public static T GetKey<T>(this IConfiguration config, string key)
        {
            var value = config[key];
            return (T) Convert.ChangeType(value, typeof(T));
        }

        public static bool GetPreferXmlOverYaml(this IConfiguration config)
        {
            return GetKey<bool>(config, "PreferXmlOverYaml");
        }
        
        public static bool GetAutoClose(this IConfiguration config)
        {
            return GetKey<bool>(config, "AutoClose");
        }
    }
}