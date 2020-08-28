using System;
using System.Collections.Generic;

namespace Mediinfo.Utility.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// list定位索引扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="startRowHandle"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int LocateByValue<T>(this List<T> list, int startRowHandle, string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return -1;
            Type entityType = typeof(T);
            for (int i = startRowHandle; i < list.Count; i++)
            {
                try
                {
                    System.Reflection.PropertyInfo proInfo = entityType.GetProperty(propertyName.ToUpper());
                    if (proInfo != null)
                    {
                        object result = proInfo.GetValue(list[i], null);
                        if (result.ToStringEx().Equals(value))
                            return i;
                    }
                }
                catch (Exception)
                {
                    return -1;
                }
            }

            return -1;
        }
    }
}
