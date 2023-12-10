using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace TheKrystalShip.RadioBot.Tools
{
    public static class AppSettings
    {
        private const string SETTINGS_FILE = "appsettings.json";
        private static readonly IConfiguration _config;

        static AppSettings()
        {
            if (!File.Exists(SETTINGS_FILE))
            {
                throw new FileNotFoundException($"File '{SETTINGS_FILE}' could not be found");
            }

            _config = new ConfigurationBuilder()
                    .AddJsonFile(SETTINGS_FILE, optional: false, reloadOnChange: true)
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
