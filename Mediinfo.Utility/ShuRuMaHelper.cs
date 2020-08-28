using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mediinfo.Utility
{
    /// <summary>
    /// 输入码工具类
    /// </summary>
    public class ShuRuMa
    {
        /// <summary>
        /// 汉字
        /// </summary>
        public string HANZI { get; set; }

        /// <summary>
        /// 拼音
        /// </summary>
        public string QUANPIN { get; set; }

        /// <summary>
        /// 输入码1
        /// </summary>
        public string SHURUMA1 { get; set; }

        /// <summary>
        ///  输入码2
        /// </summary>
        public string SHURUMA2 { get; set; }

        /// <summary>
        ///  输入码3
        /// </summary>
        public string SHURUMA3 { get; set; }

        /// <summary>
        /// 多音标识
        /// </summary>
        public long? DUOYINBZ { get; set; }
    }

    /// <summary>
    /// 多音字
    /// </summary>
    public class DuoYiZi
    {
        /// <summary>
        /// 汉字
        /// </summary>
        public string HANZI { get; set; }

        /// <summary>
        /// 词组
        /// </summary>
        public string CIZU { get; set; }

        /// <summary>
        /// 当前位置
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// 输入码
        /// </summary>
        public string SHURUMA1 { get; set; }
    }

    /// <summary>
    /// 输入码帮助类
    /// </summary>
    public class ShuRuMaHelper
    {
        private static readonly Dictionary<string, ShuRuMa> HanZiKu = new Dictionary<string, ShuRuMa>();
        private static readonly List<DuoYiZi> DuoYinKu;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        static ShuRuMaHelper()
        {
            var list = ReadCodeFile();
            list.ForEach(o =>
            {
                if (!HanZiKu.ContainsKey(o.HANZI))
                {
                    HanZiKu.Add(o.HANZI, o);
                }
            });
            DuoYinKu = ReadCode2File();
            DuoYinKu.ForEach(o =>
            {
                o.CurrentIndex = o.CIZU.IndexOf(o.HANZI, StringComparison.Ordinal);
            });
        }

        public static void InitializeCache()
        {

        }

        /// <summary>
        /// 获取输入码
        /// </summary>
        /// <param name="word">中文</param>
        /// <param name="shuRuMa1">拼音码</param>
        /// <param name="shuRuMa2">五笔码</param>
        /// <param name="shuRuMa3">自定义码</param>
        /// <param name="defaultLength">输入码长度</param>
        /// <returns></returns>
        public static List<ShuRuMa> GetShuRuMa(string word, out string shuRuMa1, out string shuRuMa2,
                                               out string shuRuMa3, int defaultLength = 10)
        {
            List<ShuRuMa> hanZiKus = new List<ShuRuMa>();
            Regex regEnglish = new Regex("^[a-zA-Z]");
            word.ToList().ForEach(o =>
            {
                var index = o.ToString();
                if (regEnglish.IsMatch(index)) index = index.ToUpper();//判断如果输入的字符是字母，先将该字母转为大写
                if (HanZiKu.ContainsKey(index))
                {
                    var hanzi = HanZiKu[index];
                    var shuruma = string.Empty;
                    hanZiKus.Add(new ShuRuMa() { HANZI = index, QUANPIN = hanzi.QUANPIN, SHURUMA1 = (string.IsNullOrWhiteSpace(shuruma) ? hanzi.SHURUMA1 : shuruma), SHURUMA2 = hanzi.SHURUMA2, SHURUMA3 = hanzi.SHURUMA3 });
                }
            });
            foreach (var duoyinzi in DuoYinKu)
            {
                var index = word.IndexOf(duoyinzi.CIZU, StringComparison.Ordinal);
                if (index > -1)
                {
                    hanZiKus[index + duoyinzi.CurrentIndex].SHURUMA1 = duoyinzi.SHURUMA1;
                    break;
                }
            }

            shuRuMa1 = string.Empty;
            shuRuMa2 = string.Empty;
            shuRuMa3 = string.Empty;
            for (int i = 0; i < hanZiKus.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(hanZiKus[i].QUANPIN))
                {
                    if (i < defaultLength)
                    {
                        shuRuMa1 += hanZiKus[i].SHURUMA1;
                        shuRuMa2 += hanZiKus[i].SHURUMA2;
                        shuRuMa3 += hanZiKus[i].SHURUMA3;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return hanZiKus;
        }

        /// <summary>
        /// 写入json文件
        /// </summary>
        /// <param name="HanZiKus">输入码列表</param>
        public void WriterTOJsonFile(List<ShuRuMa> HanZiKus)
        {
            JsonSerializer serializer = new JsonSerializer();
            Console.WriteLine("AppDomain.CurrentDomain.BaseDirectory:" + AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("System.IO.Directory.GetCurrentDirectory():" + System.IO.Directory.GetCurrentDirectory());
            Console.WriteLine("System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase:" + System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "ShuRuMa.json", FileMode.Create);
            file.Seek(0, SeekOrigin.Begin);
            StreamWriter writer = new StreamWriter(file);
            StringWriter sw = null;
            HanZiKus.ForEach(o =>
            {
                sw = new StringWriter();
                serializer.Serialize(new JsonTextWriter(sw), o);
                writer.WriteLine(sw.GetStringBuilder().ToString());
            });
            sw.Flush();
            sw.Close();
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// 写入json文件
        /// </summary>
        /// <param name="DuoYiZis">多音字</param>
        public void WriterTOJsonFile(List<DuoYiZi> DuoYiZis)
        {
            JsonSerializer serializer = new JsonSerializer();
            FileStream file = new FileStream("DuoYinZi.json", FileMode.Create);
            file.Seek(0, SeekOrigin.Begin);
            StreamWriter writer = new StreamWriter(file);
            StringWriter sw = null;
            DuoYiZis.ForEach(o =>
            {
                sw = new StringWriter();
                serializer.Serialize(new JsonTextWriter(sw), o);
                writer.WriteLine(sw.GetStringBuilder().ToString());
            });
            // 清空缓冲区、关闭流
            sw.Flush();
            sw.Close();
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// 读取json
        /// </summary>
        /// <returns>多音字</returns>
        static List<DuoYiZi> ReadCode2File()
        {
            List<DuoYiZi> hanZiKus2 = new List<DuoYiZi>();
            JsonSerializer serializer = new JsonSerializer();
            Console.WriteLine("AppDomain.CurrentDomain.BaseDirectory:" + AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("System.IO.Directory.GetCurrentDirectory():" + Directory.GetCurrentDirectory());
            Console.WriteLine("System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase:" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

            using StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "DuoYinZi.json");
            while (!reader.EndOfStream)
            {
                StringReader sr = new StringReader(reader.ReadLine() ?? string.Empty);
                hanZiKus2.Add((DuoYiZi)serializer.Deserialize(new JsonTextReader(sr), typeof(DuoYiZi)));
            }

            return hanZiKus2;
        }

        /// <summary>
        /// 读取json
        /// </summary>
        /// <returns>输入码</returns>
        static List<ShuRuMa> ReadCodeFile()
        {
            List<ShuRuMa> HanZiKus = new List<ShuRuMa>();
            JsonSerializer serializer = new JsonSerializer();
            using StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "ShuRuMa.json");
            while (!reader.EndOfStream)
            {
                StringReader sr = new StringReader(reader.ReadLine() ?? string.Empty);
                HanZiKus.Add((ShuRuMa)serializer.Deserialize(new JsonTextReader(sr), typeof(ShuRuMa)));
            }

            return HanZiKus;
        }
    }
}
