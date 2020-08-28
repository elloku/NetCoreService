using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mediinfo.Infrastructure.Core.Settings
{
    public class SettingContainer
    {
        private Dictionary<Type, Type> _registrations = new Dictionary<Type, Type>();
        private static readonly SettingContainer _instance = new SettingContainer();

        private SettingContainer() {

        }

        public static SettingContainer Instance
        {
            get { return _instance; }
        }

        public T Get<T>(params object[] args) where T:class
        {
            Type type;
            if (_registrations.TryGetValue(typeof(T), out type))
            {
                return CreateInstance(type, args) as T;
            }

            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

            List<Assembly> assemblys = new List<Assembly>();

            DirectoryInfo TheFolder = new DirectoryInfo(assemblyPath);
            var mediDlls = TheFolder.GetFiles().Where(c => c.Extension.Contains("dll") && (c.Name.Contains("Mediinfo.") && !c.Name.Contains("WinForm"))).ToList();

            // 遍历文件
            foreach (FileInfo NextDll in mediDlls)
            {
                var ass = Assembly.LoadFrom(NextDll.FullName);
                assemblys.Add(ass);
            }

            type = assemblys.SelectMany(a => a.GetTypes().Where(t => t.IsSubclassOf(typeof(T)))).FirstOrDefault();

            if(type != null)
            {
                _registrations.Add(typeof(T), type);
                return CreateInstance(type,args) as T;
            }
            else
            {
                throw new NotImplementedException("没有找到类的实现：" + typeof(T));
            }
        }

        private object CreateInstance(Type type, params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }

        private T CreateInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }
    }
}
