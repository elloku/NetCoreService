using System.Collections.Generic;

namespace Mediinfo.Utility
{
    /// <summary>
    /// 可按指定List中的元素去除重复 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public delegate bool EqualsComparer<F>(F x, F y);

        /// <summary>
        /// 
        /// </summary>
        public EqualsComparer<T> equalsComparer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_euqlsComparer"></param>
        public ListComparer(EqualsComparer<T> _euqlsComparer)
        {
            this.equalsComparer = _euqlsComparer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            if (null != equalsComparer)
            {
                return equalsComparer(x, y);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
