using Ionic.Zip;
using System;

namespace Mediinfo.APIGateway.Core.Generate
{
    /// <summary>
    /// zip打包
    /// </summary>
    public class ZipServiceClient
    {
        /// <summary>
        /// 打包
        /// </summary>
        /// <returns></returns>
        public static string Zip()
        {
            using (ZipFile zip = new ZipFile(System.Text.Encoding.Default))//解决中文乱码问题
            {
                string serviceName = System.Configuration.ConfigurationManager.AppSettings["ServiceName"];
                string serviceVersion = System.Configuration.ConfigurationManager.AppSettings["ServiceVersion"];
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "ServiceClient / " + serviceVersion + "/";
                string[] files = System.IO.Directory.GetFiles(filePath);
                foreach (var file in files)
                {
                    zip.AddFile(file, "");
                }
                string zipPath = filePath + serviceName + "_" + serviceVersion + ".zip";
                zip.Save(zipPath);
                return zipPath;
            }
        }
    }
}
