using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace TheKrystalShip.RadioBot.Tools
{
    public static class Configuration
    {
        private const string SETTINGS_FILE = "appsettings.json";
        private static readonly IConfiguration _config;

        static Configuration()
        {
            if (!File.Exists(SETTINGS_FILE))
                File.Create(SETTINGS_FILE);

            _config = new ConfigurationBuilder()
                    .AddJsonFile(SETTINGS_FILE, optional: false, reloadOnChange: true)
                    .Build();
        }

        public static string Get(string key)
        {
            string value = _config[key];
            Console.WriteLine(value);

            return _config[key];
        }

        public static bool OsIsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool OsIsMac() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool OsIsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
}
