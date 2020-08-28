using Mediinfo.Enterprise;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Http;

namespace Mediinfo.APIGateway.Core.Generate
{
    /// <summary>
    /// 生成代理类
    /// </summary>
    public class GenerateController : ApiController
    {
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceVersion"></param>
        public void Generate(Assembly assembly, string serviceName, string serviceVersion)
        {
            var types = assembly.GetTypes().Where(t => t.BaseType.BaseType == typeof(ApiBaseController))
    .Distinct().ToList();

            foreach (var apiType in types)
            {
                //客户端代理类模版
                StringBuilder generateContent = new StringBuilder();
                generateContent.AppendLine(@"//注意：此代码由HIS6微服务自动生成的客户端代理类，在没有确保安全的情况下，请勿随便修改！
using Mediinfo.Enterprise;
using Mediinfo.ServiceProxy.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {Namespace}
{
    public partial class {ClassName}Service
    {
        public ServiceClient serviceClient = null;
        public {ClassName}Service()
        {" +
"\n            serviceClient = new ServiceClient(\"{ServiceName}" +
                "\",\"{ServiceVersion}"+
                "\");\n" +
        "        }\n" +

        @"{Methods}

     }
}
//注意：此代码由HIS6微服务自动生成的客户端代理类，在没有确保安全的情况下，请勿随便修改！
");

                StringBuilder generateMethodContent = new StringBuilder();

                string className = apiType.Name.Replace("Controller", "");
                string nameSpace = apiType.Namespace.Replace("Mediinfo.Service.", "Mediinfo.ServiceProxy.");

                string fileName = className;
                string drlName = apiType.Namespace.Substring(apiType.Namespace.LastIndexOf('.') + 1);
                generateContent = generateContent.Replace("{ServiceName}", serviceName);
                generateContent = generateContent.Replace("{ServiceVersion}", serviceVersion);

                generateContent = generateContent.Replace("{ClassName}", className);
                generateContent = generateContent.Replace("{Namespace}", nameSpace.Replace(".Controllers",""));

                var srgType = typeof(ServiceResult<object>).GetGenericTypeDefinition();
                var methods = apiType.GetMethods().Where(m => m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == srgType);

                foreach (var method in methods)
                {
                    generateMethodContent = generateMethod(generateMethodContent, className, method , false);
                    generateMethodContent = generateMethod(generateMethodContent, className, method, true);
                }

                generateContent.Replace("{Methods}", generateMethodContent.ToString());

                string dirPath = AppDomain.CurrentDomain.BaseDirectory + "ServiceClient/" + serviceName + "/" + serviceVersion + "/" + drlName + "/";
                CreateDir(dirPath);
                CreateFile(AppDomain.CurrentDomain.BaseDirectory + "ServiceClient/" + serviceName + "/" + serviceVersion + "/" + drlName + "/" + fileName + "Service.cs", generateContent.ToString());
            }
        }

        /// <summary>
        /// 生成方法
        /// </summary>
        /// <param name="generateMethodContent"></param>
        /// <param name="fileName"></param>
        /// <param name="method"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        private StringBuilder generateMethod(StringBuilder generateMethodContent, string fileName, MethodInfo method, bool async)
        {
            if (async)
            {
                generateMethodContent.AppendLine(@"        public async Task<Result<{ReturnType}>> {MethodName}Async({Parameters})
        {" +
                           "\n            return await serviceClient.InvokeAsync<{ReturnType}>(\"{ClassName}\", \"{MethodName}\"" +
                                @"{ServiceParameters});
        }");
            }
            else
            {
                generateMethodContent.AppendLine(@"        public Result<{ReturnType}> {MethodName}({Parameters})
        {" +
                           "\n            return serviceClient.Invoke<{ReturnType}>(\"{ClassName}\", \"{MethodName}\"" +
                                @"{ServiceParameters});
        }");
            }
            // 客户端代理类方法内容
            generateMethodContent = generateMethodContent.Replace("{ClassName}", fileName);

            string methodName = method.Name;
            generateMethodContent = generateMethodContent.Replace("{MethodName}", methodName);

            // 计算出返回值的类型，注意：这里要考虑泛型！
            Type returnType = method.ReturnType.GetGenericArguments()[0];
            StringBuilder returnTypeName = new StringBuilder();
            if (returnType.IsGenericType)
            {
                returnTypeName.Append(GetGenericTypeName(returnType));
            }
            else
            {
                returnTypeName.Append(returnType.FullName);
            }
            generateMethodContent = generateMethodContent.Replace("{ReturnType}", returnTypeName.ToString());

            var parms = method.GetParameters();
            StringBuilder parameters = new StringBuilder();
            StringBuilder serviceParameters = new StringBuilder();
            foreach (var parm in parms)
            {
                // 生成参数
                var pName = parm.Name;
                var pType = parm.ParameterType;
                var defaultValue = parm.DefaultValue;

                StringBuilder pTypeValue = new StringBuilder();
                if (pType.IsGenericType)
                {
                    pTypeValue.Append(GetGenericTypeName(pType));
                }
                else
                {
                    pTypeValue.Append(pType.FullName);
                }

                parameters.Append(",");
                parameters.Append(pTypeValue);
                parameters.Append(" ");
                parameters.Append(pName);

                // 把转换默认值
                if(defaultValue != null && defaultValue.GetType() != typeof(DBNull))
                {
                    if (defaultValue is string)
                    {
                        parameters.Append(" = ");
                        parameters.Append("\"" + defaultValue + "\"");
                    }
                    else if (defaultValue is bool)
                    { 
                        parameters.Append(" = ");
                        parameters.Append(defaultValue.ToString()?.ToLower());
                    }
                    else
                    {
                        parameters.Append(" = ");
                        parameters.Append(defaultValue);
                    }
                }


                serviceParameters.Append(",");
                serviceParameters.Append("new ServiceParm(nameof(" + pName + "), " + pName + ")");
            }

            // 去除开头的逗号
            if (parameters.Length > 0)
                parameters = new StringBuilder(parameters.ToString().Substring(1));

            generateMethodContent = generateMethodContent.Replace("{Parameters}", parameters.ToString());
            generateMethodContent = generateMethodContent.Replace("{ServiceParameters}", serviceParameters.ToString());
            return generateMethodContent;
        }

        /// <summary>
        /// 获取泛型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetGenericTypeName(Type type)
        {
            StringBuilder result = new StringBuilder();
            result.Append(type.GetGenericTypeDefinition().FullName.Replace("`" + type.GetGenericArguments().Length, ""));
            result.Append("<");

            var typeArguments = type.GetGenericArguments();
            for (int i = 0; i < typeArguments.Length; i++)
            {
                if (typeArguments[i].IsGenericType)
                {
                    if (i == typeArguments.Length - 1)
                    {
                        result.Append(GetGenericTypeName(typeArguments[i]));
                    }
                    else
                    {
                        result.Append(GetGenericTypeName(typeArguments[i])+",");
                    }
                }
                else
                {
                    if (i == typeArguments.Length - 1)
                    {
                        result.Append(typeArguments[i].FullName);
                    }
                    else
                    {
                        result.Append(typeArguments[i].FullName + ",");
                    }
                }
            }

            result.Append(">");

            return result.ToString();
        }

        // 根据文件夹全路径创建文件夹
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
