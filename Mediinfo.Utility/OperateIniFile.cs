using System.Runtime.InteropServices;
using System.Text;

namespace Mediinfo.Utility
{
    /// <summary>
    ///操作ini读取或者写数据库连接字符串
    /// </summary>
    public class OperateIniFile
    {
        public string FileName { get; set; }

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileInt(
            string iniAppName,
            string iniKeyName,
            int iniDefault,
            string iniFileName
            );

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(
            string iniAppName,
            string iniKeyName,
            string iniDefault,
            StringBuilder iniReturnedString,
            int nSize,
            string iniFileName
            );

        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(
            string iniAppName,
            string iniKeyName,
            string iniString,
            string iniFileName
            );

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="aFileName">Ini文件路径</param>
        public OperateIniFile(string aFileName)
        {
            this.FileName = aFileName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public OperateIniFile()
        {

        }

        /// <summary>
        /// [扩展]读Int数值
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public int ReadInt(string section, string name, int def)
        {
            return GetPrivateProfileInt(section, name, def, this.FileName);
        }

        /// <summary>
        /// [扩展]读取string字符串
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public string ReadString(string section, string name, string def)
        {
            StringBuilder vRetSb = new StringBuilder(2048);
            GetPrivateProfileString(section, name, def, vRetSb, 2048, this.FileName);
            return vRetSb.ToString();
        }

        /// <summary>
        /// [扩展]写入Int数值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="Ival">写入值</param>
        public void WriteInt(string section, string name, int Ival)
        {
            WritePrivateProfileString(section, name, Ival.ToString(), this.FileName);
        }

        /// <summary>
        /// [扩展]写入String字符串，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="strVal">写入值</param>
        public void WriteString(string section, string name, string strVal)
        {
            WritePrivateProfileString(section, name, strVal, this.FileName);
        }

        /// <summary>
        /// 删除指定的 节
        /// </summary>
        /// <param name="section"></param>
        public void DeleteSection(string section)
        {
            WritePrivateProfileString(section, null, null, this.FileName);
        }

        /// <summary>
        /// 删除全部 节
        /// </summary>
        public void DeleteAllSection()
        {
            WritePrivateProfileString(null, null, null, this.FileName);
        }

        /// <summary>
        /// 读取指定 节-键 的值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string IniReadValue(string section, string name)
        {
            StringBuilder strSb = new StringBuilder(256);
            GetPrivateProfileString(section, name, "", strSb, 256, this.FileName);
            return strSb.ToString();
        }

        /// <summary>
        /// 写入指定值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void IniWriteValue(string section, string name, string value)
        {
            WritePrivateProfileString(section, name, value, this.FileName);
        }
    }
}
