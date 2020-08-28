using Mediinfo.Enterprise;
using Mediinfo.Enterprise.Exceptions;

namespace Mediinfo.Infrastructure.Core.Controller
{
    /// <summary>
    /// 服务校验的实现
    /// </summary>
    public class ServiceCheck : ICheck
    {
        public void IsFalse(bool value, string message)
        {
            if (value == true)
            {
                throw new ServiceCheckException(message);
            }
        }

        public void IsTrue(bool value, string message)
        {
            if (value == false)
            {
                throw new ServiceCheckException(message);
            }
        }

        public string NotEmpty(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ServiceCheckException(message);
            }
            return value;
        }

        public T NotNull<T>(T value, string message) where T : class
        {
            if (value == null)
            {
                throw new ServiceCheckException(message);
            }
            return value;
        }

        public T? NotNull<T>(T? value, string message) where T : struct
        {
            if (value == null)
            {
                throw new ServiceCheckException(message);
            }
            return value;
        }
    }
}
