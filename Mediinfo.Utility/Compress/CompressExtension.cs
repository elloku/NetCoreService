using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Mediinfo.Utility.Compress
{
    /// <summary>
    /// 对数据压缩的扩展
    /// </summary>
    public static class CompressExtension
    {
        /// <summary>
        /// 转换为压缩数据
        /// </summary>
        /// <param name="obj">待压缩的数据</param>
        /// <returns></returns>
        public static string ToCompress(this object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            byte[] rawData = Encoding.UTF8.GetBytes(json);
            byte[] zippedData = Compress(rawData);
            return Convert.ToBase64String(zippedData);
        }

        public static string CompressString(this string json)
        {
            byte[] rawData = Encoding.UTF8.GetBytes(json);
            byte[] zippedData = Compress(rawData);
            return Convert.ToBase64String(zippedData);
        }

        /// <summary>
        /// 解压数据
        /// </summary>
        /// <typeparam name="T">解压后的类型</typeparam>
        /// <param name="zippedBase64Data">待解压数据</param>
        /// <returns></returns>
        public static T Decompress<T>(this string zippedBase64Data)
        {
            if (zippedBase64Data == "没有权限！")
            {
                throw new ApplicationException("没有权限！");
            }

            byte[] zippedData = Convert.FromBase64String(zippedBase64Data);
            byte[] dezippedData = Decompress(zippedData);
            string strData = Encoding.UTF8.GetString(dezippedData);
            return JsonConvert.DeserializeObject<T>(strData);
        }

        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="zippedBase64Data">待解压数据</param>
        /// <param name="type">解压类型</param>
        /// <returns></returns>
        public static object Decompress(this string zippedBase64Data, Type type)
        {
            if (zippedBase64Data == "没有权限！")
            {
                throw new ApplicationException("没有权限！");
            }

            byte[] zippedData = Convert.FromBase64String(zippedBase64Data);
            byte[] dezippedData = Decompress(zippedData);
            string strData = Encoding.UTF8.GetString(dezippedData);
            return JsonConvert.DeserializeObject(strData, type);
        }

        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="zippedBase64Data">待解压数据</param>
        /// <returns></returns>
        public static string DecompressString(this string zippedBase64Data)
        {
            if (zippedBase64Data == "没有权限！")
            {
                throw new ApplicationException("没有权限！");
            }

            byte[] zippedData = Convert.FromBase64String(zippedBase64Data);
            byte[] dezippedData = Decompress(zippedData);
            string strData = Encoding.UTF8.GetString(dezippedData);
            return strData;

        }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="rawData">待压缩数据</param>
        /// <returns></returns>
        private static byte[] Compress(byte[] rawData)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            byte[] result = ms.ToArray();
            ms.Dispose();
            ms.Close();
            return result;
        }

        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="zippedData">待解压数据</param>
        /// <returns></returns>
        private static byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            ms.Dispose();
            ms.Close();
            return outBuffer.ToArray();

        }
        /// <summary>
        /// 将64位字节流解压后反序列化成object对象
        /// </summary>
        /// <param name="binaryData">字节数组</param>
        /// <returns>object对象</returns>
        public static object Base64ToObjDeCompress(this string stream)
        {
            //转成64位字节
            var compressBeforeByte = Convert.FromBase64String(stream);
            //解压缩
            var bytes = Decompress(compressBeforeByte);
            MemoryStream memStream = new MemoryStream(bytes);
            IFormatter brFormatter = new BinaryFormatter();
            Object obj = brFormatter.Deserialize(memStream);
            memStream.Close();
            memStream.Dispose();
            return obj;
        }
        /// <summary>
        /// 将数据压缩并转化成64位字节流
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToBase64Compress(this object obj)
        {
            byte[] bytes = GetBinaryFormatDataCompress(obj);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 将objec格式化成字节数组byte[]，并压缩
        /// </summary>
        /// <param name="dsOriginal">object对象</param>
        /// <returns>字节数组</returns>
        private static byte[] GetBinaryFormatDataCompress(object dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return Compress(binaryDataResult);
        }
    }
}
