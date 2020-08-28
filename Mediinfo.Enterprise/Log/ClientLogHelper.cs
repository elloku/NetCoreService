using Mediinfo.Enterprise.Config;

using System;
using System.IO;
using System.Text;

namespace Mediinfo.Enterprise.Log
{
    /// <summary>
    /// 客户端日志类
    /// </summary>
    public class ClientLogHelper
    {
        #region constructor

        private ClientLogHelper()
        {
            DateTime dateTime = DateTime.Now;
            string path = Environment.CurrentDirectory + "\\log\\" + dateTime.ToString("yyyy") + "\\" + dateTime.ToString("yyyyMM");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = path + "\\" + dateTime.ToString("yyyyMMdd") + "{0}.txt";
            int num = 1;
            while (File.Exists(string.Format(fileName, num.ToString().PadLeft(3, '0'))))
            {
                num++;
            }
            fileName = string.Format(fileName, num.ToString().PadLeft(3, '0'));
            _fileStream = new FileStream(fileName, FileMode.Create);
            _streamWriter = new StreamWriter(_fileStream, Encoding.Unicode);
        }

        #endregion

        #region fields

        private static FileStream _fileStream;
        private static StreamWriter _streamWriter;

        #endregion

        #region properties

        public static ClientLogHelper Intance { get; } = new ClientLogHelper();

        private string LogFlag
        {
            get
            {
                return MediinfoConfig.GetValue("SystemConfig.xml", "WriteLog");
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// 写入实时日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void WriteLog(string msg = "")
        {
            if (!String.IsNullOrWhiteSpace(LogFlag) && LogFlag.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                if (_streamWriter != null)
                {
                    _streamWriter.WriteLine("记录时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + msg);
                    _streamWriter.Flush();
                }
            }
        }

        #endregion

        #region destructor

        /// <summary>
        /// 析构函数
        /// </summary>
        ~ClientLogHelper()
        {
            if (_streamWriter != null)
            {
                _streamWriter.Flush();
                _streamWriter.Close();
            }
            if (_fileStream != null)
            {
                _fileStream.Flush();
                _fileStream.Close();
            }
        }

        #endregion
    }
}
