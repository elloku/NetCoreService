using System.Text;

namespace Mediinfo.Utility
{
    public class TelePhoneNoValidation
    {
        /// <summary>
        /// 手机号码验证
        /// </summary>
        /// <param name="telePhoneNo">手机号码</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool ChectTelePhoneNo(string telePhoneNo, ref string errMsg)
        {
            if (telePhoneNo != null && telePhoneNo.Length == 11)
            {
                if (IsNuber(telePhoneNo))
                {
                    return true;
                }
                else
                {
                    errMsg = "号码应为纯数字";
                    return false;
                }
            }
            else
            {
                errMsg = "号码格式有误，应为11位号码";
                return false;
            }
        }

        /// <summary>
        /// 验证是否为数字
        /// </summary>
        /// <param name="telePhoneNo"></param>
        /// <returns></returns>
        private static bool IsNuber(string telePhoneNo)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytestr = ascii.GetBytes(telePhoneNo);

            foreach (byte c in bytestr)
            {
                if (c < 48 || c > 57)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
