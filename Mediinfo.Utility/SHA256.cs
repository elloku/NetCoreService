using System;
using System.Security.Cryptography;
using System.Text;

namespace Mediinfo.Utility
{
    /// <summary>
    /// SHA256加密
    /// </summary>
    public class SHA256
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="strIN">加密字符</param>
        /// <returns></returns>
        public static string Encrypt(string strIN)
        {
            SHA256Managed sha256 = new SHA256Managed();
            
            var tmpByte = sha256.ComputeHash(GetKeyByteArray(strIN));
            sha256.Clear();

            return Convert.ToBase64String(tmpByte);
        }

        /// <summary>
        /// 字节与字符串的转换
        /// </summary>
        /// <param name="Byte"></param>
        /// <returns></returns>
        private static string GetStringValue(byte[] Byte)
        {
            ASCIIEncoding asc = new ASCIIEncoding();
            var tmpString = asc.GetString(Byte);
            return tmpString;
        }

        /// <summary>
        /// 字符串与字节的转换
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        private static byte[] GetKeyByteArray(string strKey)
        {
            ASCIIEncoding asc = new ASCIIEncoding();

            var tmpByte = asc.GetBytes(strKey);

            return tmpByte;
        }
    }
}
