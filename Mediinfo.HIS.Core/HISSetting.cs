using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// HIS配置文件
    /// </summary>
    public class HISSetting
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public HISSetting()
        {
            ConfigFile = string.Empty;
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        public static T Load<T>(string file) where T : HISSetting, new()
        {
            if (string.IsNullOrWhiteSpace(file))
                return null;

            if (!File.Exists(file))
            {
                T t = new T();
                t.ConfigFile = file;
            }

            try
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    T ret = (T)xs.Deserialize(reader);

                    ret.ConfigFile = file;
                    return ret;
                }
            }
            catch
            {
                T t = new T();
                t.ConfigFile = file;
                return t;
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        public virtual void Save()
        {
            Type type = this.GetType();

            using (StreamWriter sw = new StreamWriter(ConfigFile))
            {
                XmlSerializer xs = new XmlSerializer(type);

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                xs.Serialize(sw, this, ns);
            }
        }

        /// <summary>
        /// 配置文件名称
        /// </summary>
        [XmlIgnore]
        public string ConfigFile { get; set; }

        /// <summary>
        /// 配置项节点列表
        /// </summary>
        [XmlArray(ElementName = "ConfigSections"), XmlArrayItem("ConfigSection")]
        public List<HISConfigSection> ConfigSections { get; set; }

        /// <summary>
        /// 读取配置文件（不区分大小写）,如果不存在则返回默认值
        /// </summary>
        /// <param name="sectionName">节点名称（不区分大小写）</param>
        /// <param name="itemName">配置项名称（不区分大小写）</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public virtual string GetConfigItemValue(string sectionName, string itemName, string defaultValue = "")
        {
            if (null == ConfigSections || ConfigSections.Count <= 0 || string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(sectionName))
                return defaultValue;

            var section = ConfigSections.Where(c => c.SectionName == sectionName.ToUpper()).FirstOrDefault();
            if (null == section || section.ConfigItems == null || section.ConfigItems.Count == 0)
                return defaultValue;

            var item = section.ConfigItems.Where(c => c.ItemName == itemName.ToUpper()).FirstOrDefault();

            if (null == item)
                return defaultValue;

            return item.ItemValue;
        }

        /// <summary>
        /// 写入配置文件（不区分大小写）
        /// </summary>
        /// <param name="sectionName">节点名称（不区分大小写）</param>
        /// <param name="itemName">配置项名称（不区分大小写）</param>
        /// <param name="itemValue">配置项的值</param>
        /// <returns></returns>
        public virtual int SetConfigItemValue(string sectionName, string itemName, string itemValue)
        {
            if (string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(sectionName))
                return -1;

            if (null == ConfigSections)
                ConfigSections = new List<HISConfigSection>();

            var section = ConfigSections.Where(c => c.SectionName == sectionName.ToUpper()).FirstOrDefault();
            if (null == section || null == section.ConfigItems)
            {
                // 添加节点
                section = new HISConfigSection
                {
                    SectionName = sectionName.ToUpper(),
                    ConfigItems = new List<HISConfigItem>()
                };

                // 添加项目
                section.ConfigItems.Add(new HISConfigItem
                {
                    ItemName = itemName.ToUpper(),
                    ItemValue = itemValue
                });

                ConfigSections.Add(section);

                return 0;
            }

            var item = section.ConfigItems.Where(c => c.ItemName == itemName.ToUpper()).FirstOrDefault();

            if (null == item)
            {
                // 添加节点
                section.ConfigItems.Add(new HISConfigItem
                {
                    ItemName = itemName.ToUpper(),
                    ItemValue = itemValue
                });
            }
            else
            {
                item.ItemValue = itemValue;
            }

            return 0;
        }
    }

    //[XmlRoot("ConfigSection")]
    public class HISConfigSection
    {
        public HISConfigSection()
        {
            //ConfigItems = new List<HISConfigItem>();
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "Name")]
        public string SectionName { get; set; }

        [XmlArray("ConfigItems"), XmlArrayItem("Item")]
        public List<HISConfigItem> ConfigItems { get; set; }
    }

    public class HISConfigItem
    {
        [XmlAttribute(AttributeName = "Name")]
        public string ItemName { get; set; }

        [XmlAttribute(AttributeName = "Value")]
        public string ItemValue { get; set; }
    }
}
