using Microsoft.Extensions.Configuration;

using System.IO;

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
    }
}
