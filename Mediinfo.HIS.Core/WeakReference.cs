using System.Runtime.InteropServices;

namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakReference<T>
    {
        private GCHandle _handle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public WeakReference(T obj)
        {
            if (obj == null) return;
            _handle = GCHandle.Alloc(obj, GCHandleType.Weak);
        }

        /// <summary>
        /// 引用的目标是否还存活(没有被GC回收)
        /// </summary>
        public bool IsAlive
        {
            get { return _handle != default(GCHandle) && _handle.Target != null; }
        }

        /// <summary>
        /// 引用的目标
        /// </summary>
        public T Target
        {
            get
            {
                if (_handle == default(GCHandle)) return default(T);
                return (T)_handle.Target;
            }
        }

        /// <summary>
        /// 释放弱引用
        /// </summary>
        ~WeakReference()
        {
            if (_handle.IsAllocated)
                _handle.Free();
        }
    }
}