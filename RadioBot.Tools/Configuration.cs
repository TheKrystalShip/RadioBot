using Microsoft.Extensions.Configuration;

using System.IO;
using System.Runtime.InteropServices;

namespace TheKrystalShip.RadioBot.Tools
{
    public static class Configuration
    {
        private static readonly string _settingsFile = Path.Combine("Properties", "settings.json");
        private static readonly IConfiguration _config;

        static Configuration()
        {
            Directory.CreateDirectory("Properties");

            if (!File.Exists(_settingsFile))
                File.Create(_settingsFile);

            _config = new ConfigurationBuilder()
                    .AddJsonFile(_settingsFile, optional: false, reloadOnChange: true)
                    .Build();
        }

        public static string Get(string key)
        {
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
