using System;

namespace Mediinfo.Utility
{
    /// <summary>
    /// 出生日期验证
    /// </summary>
    /// <returns></returns>
    public class BirthdayValidation
    {
        /// <summary>
        /// 出生日期验证
        /// </summary>
        /// <param name="dateTime">出生日期</param>
        /// <param name="age">年龄</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool CheckBirthday(DateTime dateTime, ref int age, ref string errMsg)
        {
            DateTime now = DateTime.Now;
            age = now.Year - dateTime.Year;
            if (now.Month < dateTime.Month || (now.Month == dateTime.Month && now.Day < dateTime.Day))
            {
                age--;
            }
            if (age < 0)
            {
                age = 0;
                errMsg += "出生日期不能大于当前时间";
                return false;
            }

            if (age > 120)
            {
                age = 0;
                errMsg += "年龄大于120岁";
                return false;
            }
            return true;
        }
    }
}
