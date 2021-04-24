using System;
using Microsoft.Extensions.Configuration;

namespace A01.Configuration
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
            return config.GetKey<bool>("PreferXmlOverYaml");
        }
        
        public static bool GetAutoClose(this IConfiguration config)
        {
            return config.GetKey<bool>("AutoClose");
        }
    }
}