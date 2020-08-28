using System.Configuration;
using System.IO;

namespace Mediinfo.Utility
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public class ConfigHelper
    {
        private static readonly Configuration Config;
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static ConfigHelper()
        {
            var gconfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = gconfig.AppSettings;
            if (settings.Settings["PersonalSettings"] == null)
            {
                settings.Settings.Add("PersonalSettings", "PersonalSettings.config");
                gconfig.Save(ConfigurationSaveMode.Full);
            }
            if (!File.Exists(settings.Settings["PersonalSettings"].Value))
            {
                File.Create(settings.Settings["PersonalSettings"].Value);
            }

            ExeConfigurationFileMap filemap = new ExeConfigurationFileMap { ExeConfigFilename = settings.Settings["PersonalSettings"].Value };

            Config = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);
        }

        /// <summary>
        /// 获取配置节
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string Get(string Key)
        {
            if (Config.AppSettings.Settings[Key] != null)
            {
                return Config.AppSettings.Settings[Key].Value;
            }
            return null;
        }

        /// <summary>
        /// 添加一个配置
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void Add(string Key, string Value)
        {
            if (Config.AppSettings.Settings[Key] != null)
            {
                Config.AppSettings.Settings.Remove(Key);
            }
            Config.AppSettings.Settings.Add(Key, Value);
            Config.Save(ConfigurationSaveMode.Full);
        }

        /// <summary>
        /// 移除一个配置
        /// </summary>
        /// <param name="Key"></param>
        public static void Remove(string Key)
        {
            if (Config.AppSettings.Settings[Key] != null)
            {
                Config.AppSettings.Settings.Remove(Key);
            }
            Config.Save(ConfigurationSaveMode.Full);
        }
    }
}
