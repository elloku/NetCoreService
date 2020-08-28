using Mediinfo.Enterprise;
using Mediinfo.Enterprise.Exceptions;

namespace Mediinfo.Infrastructure.Core.DomainService
{
    /// <summary>
    /// DomainService校验的实现
    /// </summary>
    public class DomainServiceCheck : ICheck
    {
        public void IsFalse(bool value, string message)
        {
            if (value == true)
            {
                throw new DomianServiceCheckException(message);
            }
        }

        public void IsTrue(bool value, string message)
        {
            if (value == false)
            {
                throw new DomianServiceCheckException(message);
            }
        }

        public string NotEmpty(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomianServiceCheckException(message);
            }

            return value;
        }

        public T NotNull<T>(T value, string message) where T : class
        {
            if (value == null)
            {
                throw new DomianServiceCheckException(message);
            }

            return value;
        }

        public T? NotNull<T>(T? value, string message) where T : struct
        {
            if (value == null)
            {
                throw new DomianServiceCheckException(message);
            }

            return value;
        }
    }
}
