using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrCore.Datamodel
{
    public static class ConfigurationReader
    {
        public static System.Drawing.Color ReadHighlightColor(this IConfiguration config)
        {
            var r = config.GetValue<int>("Highlighting:Color:r");
            var g = config.GetValue<int>("Highlighting:Color:g");
            var b = config.GetValue<int>("Highlighting:Color:b");
            var a = config.GetValue<int>("Highlighting:Color:a");

            return System.Drawing.Color.FromArgb(a, r, g, b);
        }

        public static int ReadHighlightTimeout(this IConfiguration config)
        {
           return config.GetValue<int>("Highlighting:Timeout");
        }

        public static string ReadConnectionString(this IConfiguration config)
        {
            return config["Database:ConnectionString"] ?? "Connection string was empty";
        }
    }
}
