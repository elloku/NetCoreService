namespace Mediinfo.Enterprise
{
    public interface ICheck
    {
        /// <summary>
        /// 断言value一定为true，如果不是会抛出相应的异常
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        void IsTrue(bool value, string message);

        /// <summary>
        /// 断言value一定为false，如果不是会抛出相应的异常
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        void IsFalse(bool value, string message);

        /// <summary>
        /// 断言value不为空，如果为空则抛出相应异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        T NotNull<T>(T value, string message) where T : class;

        /// <summary>
        /// 断言value不为空，如果为空则抛出相应异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        T? NotNull<T>(T? value, string message) where T : struct;

        /// <summary>
        /// 断言value不为空字符，如果为空字符则抛出相应异常
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        string NotEmpty(string value, string message);
    }
}
