using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Mediinfo.Utility.Util
{
    /// <summary>
    /// xml工具类
    /// </summary>
    public class XmlUtility
    {
        /// <summary>
        /// 将自定义对象序列化为XML字符串
        /// </summary>
        /// <param name="myObject">自定义对象实体</param>
        /// <returns>序列化后的XML字符串</returns>
        public static string SerializeToXml<T>(T myObject)
        {
            if (myObject != null)
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));

                MemoryStream stream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8) {Formatting = Formatting.None}; 
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] {new XmlQualifiedName(string.Empty, "aa")});
                xs.Serialize(writer, myObject, namespaces);

                stream.Position = 0;
                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.Append(line);
                    }
                    reader.Close();
                }
                writer.Close();
                return sb.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">XML字符</param>
        /// <returns></returns>
        public static T DeserializeToObject<T>(string xml)
        {
            using MemoryStream ms = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            ms.Write(bytes, 0, bytes.Length);
            ms.Flush();
            ms.Position = 0;
            XmlSerializer xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(ms);
        }
    }
}