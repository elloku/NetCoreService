using Mediinfo.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Mediinfo.Enterprise.Config
{
    public class MediinfoConfig
    {

        private MediinfoConfig()
        {
        }

        private static readonly Dictionary<string, XmlElement> configures = new Dictionary<string, XmlElement>();
        private static readonly Dictionary<string, XElement> dicConfig = new Dictionary<string, XElement>();

        private static readonly object _lock = new object();


        /// <summary>
        /// 获取节点的值
        /// </summary>
        /// <param name="configName">配置文件名称</param>
        /// <param name="node">节点</param>
        /// <returns>节点的值</returns>
        public static string GetValue(string configName, string node)
        {
            if (!configures.ContainsKey(configName))
            {
                lock (_lock)
                {
                    string config = IOHelper.Read(AppDomain.CurrentDomain.BaseDirectory + configName);
                    // 如果未找到文件则返回空
                    if (config == null)
                        return "";
                    XmlDocument xml = new XmlDocument();
                    // 打开现有的一个xml文件
                    xml.LoadXml(config);
                    // 获得xml的根节点
                    var configure = xml.DocumentElement;
                    configures.Add(configName, configure);
                }
            }
            var snode = configures[configName].SelectSingleNode(node);
            if (snode != null)
            {
                return snode.InnerText;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取节点属性值
        /// </summary>
        /// <param name="configName">配置文件名称</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性</param>
        /// <returns>节点中属性的值</returns>
        public static string GetAttribute(string configName, string node, string attribute)
        {
            if (!dicConfig.ContainsKey(configName))
            {
                lock (_lock)
                {
                    if (!dicConfig.ContainsKey(configName))
                    {
                        lock (_lock)
                        {
                            // 打开现有的一个xml文件
                            XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + configName);
                            // 获得xml的根节点
                            var configure = doc.Element("config");
                            dicConfig.Add(configName, configure);
                        }
                    }
                }
            }
            var locNode = dicConfig[configName].Element("customer")?.Elements(node)?.FirstOrDefault();
            if (locNode != null)
            {
                return locNode.Attribute(attribute).Value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取程序的基目录
        /// </summary>
        /// <returns></returns>
        public static string BaseDirectory()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(baseDirectory + "WinFormMain.xml"))
            {
                baseDirectory = Directory.GetCurrentDirectory() + "\\";
            }
            return baseDirectory;
        }

        /// <summary>
        /// 设置节点值，不加载缓存
        /// </summary>
        /// <param name="path"></param>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetElementValueNoCache(string path, string elementName, string value)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("config")?.ChildNodes;
                if (nodeList != null)
                    foreach (XmlNode xn in nodeList) //遍历所有子节点
                    {
                        if (xn is XmlElement xe)
                        {
                            if (xe.Name == elementName)
                            {
                                xe.InnerText = value;
                                break;
                            }
                        }
                    }

                xmlDoc.Save(path);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读配置文件，类似ini文件
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Read(string strFilePath, string section, string key)
        {
            if (!File.Exists(strFilePath))
            {
                return "";
            }

            string[] iniItems = new string[0];
            string iniLines = "";
            string iniLine = "";
            int i, j;

            try
            {
                StreamReader iniFile = new StreamReader(strFilePath, Encoding.UTF8);
                string line = null;
                while ((line = iniFile.ReadLine()) != null && line.Trim() != "")
                {
                    iniLines += line + "|";
                }

                if (iniLines.Length > 0)
                {
                    iniLines = iniLines.Substring(0, iniLines.Length - 1);
                }

                iniFile.Close();

                iniFile = null;
            }
            catch
            {
                return "";
            }
            iniItems = iniLines.Split(new char[] { '|' });

            for (i = 0; i < iniItems.GetLength(0); i++)
            {
                if (iniItems[i].Trim().ToUpper() == '[' + section.Trim().ToUpper() + ']')
                {
                    for (j = i + 1; j < iniItems.GetLength(0); j++)
                    {
                        iniLine = iniItems[j].Trim();

                        if (iniLine.Length > 0)
                        {
                            if (iniLine[0] == '[' && iniLine[iniLine.Length - 1] == ']')
                                return "";
                        }

                        if (iniLine.Substring(0, Math.Min(key.Length + 1, iniLine.Length)).ToUpper() == key.ToUpper() + "=")
                        {
                            return iniItems[j].Substring(iniItems[j].IndexOf('=') + 1);
                        }
                    }

                    return "";
                }
            }

            return "";
        }

    }
}
