using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mediinfo.APIGateway.Core.Generate
{
    /// <summary>
    /// 生成代理类
    /// </summary>
    public class GenerateJS
    {
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceVersion"></param>
        public void Generate(Assembly assembly)
        {
            string serviceName = assembly.GetName().Name.TrimStart("Mediinfo.Service.".ToCharArray());
            List<Type> types = assembly.GetTypes().Where(t => t.BaseType.BaseType == typeof(ApiBaseController))
                .Distinct().ToList();

            foreach (Type apiType in types)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("//注意：此代码由HIS6微服务自动生成的客户端JS代理类，在没有确保安全的情况下，请勿随便修改！");

                // 判断Service对象是否存在
                builder.AppendLine("if (typeof Service === 'undefined' || typeof Service.ServiceClient === 'undefined') {");
                builder.AppendLine("    throw new Error(\"Service Object is not defined.\");");
                builder.AppendLine("}");

                // 创建命名空间
                string nameSpace = "Service";
                foreach (string name in serviceName.Split('.').ToList())
                {
                    builder.AppendLine();
                    builder.AppendLine(string.Format("if (!{0}.hasOwnProperty('{1}'))\n"
                        + "    {0}.{1} = {{}};", nameSpace, name));

                    nameSpace += "." + name;
                }

                // 创建服务构造函数
                string controllerName = apiType.Name.Replace("Controller", "");
                string className = string.Format("Service.{0}.{1}Service", serviceName, controllerName);

                builder.AppendLine();
                builder.AppendLine(string.Format("{0} = function() {{", className));
                builder.AppendLine(string.Format("    this.serviceClient = new Service.ServiceClient('{0}', '{1}');",
                    serviceName.Replace('.', '-'), controllerName));
                builder.AppendLine("};");

                // 创建方法
                Type srgType = typeof(ServiceResult<object>).GetGenericTypeDefinition();
                IEnumerable<MethodInfo> methods = apiType.GetMethods().Where(m => m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == srgType);
                foreach(MethodInfo method in methods)
                {
                    builder.AppendLine(GenerateMethod(method, className));
                }

                builder.AppendLine("//注意：此代码由HIS6微服务自动生成的客户端JS代理类，在没有确保安全的情况下，请勿随便修改！");

                string dirPath = AppDomain.CurrentDomain.BaseDirectory + "ServiceClient/JS/" + serviceName + "/";
                CreateDir(dirPath);
                CreateFile(dirPath + controllerName + "Service.js", builder.ToString());
            }
        }

        private string GenerateMethod(MethodInfo method, string className)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine();
            builder.Append(string.Format("{0}.prototype.{1} = function(", className, method.Name));

            // 生成参数
            StringBuilder parameters = new StringBuilder();
            StringBuilder serviceParameters = new StringBuilder();
            foreach (ParameterInfo parameterInfo in method.GetParameters())
            {
                parameters.Append(parameterInfo.Name);
                parameters.Append(", ");

                serviceParameters.AppendLine(string.Format("        {0}: {0},", parameterInfo.Name));
            }
            builder.Append(parameters.ToString().TrimEnd(", ".ToCharArray()));
            builder.AppendLine(") {");

            builder.AppendLine(string.Format("    return this.serviceClient.invoke('{0}', {{", method.Name));
            builder.AppendLine(serviceParameters.ToString().TrimEnd().TrimEnd(','));
            builder.AppendLine("    });");
            builder.Append("};");

            return builder.ToString();
        }

        //根据文件夹全路径创建文件夹
        public void CreateDir(string dirName)
        {
            string path = dirName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        public void CreateFile(string fileName, string fileContent)
        {
            TextWriter tw = new StreamWriter(fileName, false);
            tw.Write(fileContent);
            tw.Close();
        }
    }
}
