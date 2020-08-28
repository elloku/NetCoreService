using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mediinfo.Utility.CommonMethod
{
    public static class ColorTransfer
    {
        /// <summary>
        /// 把一个int值转化成rgb值
        /// </summary>
        /// <param name="yanSe"></param>
        /// <returns></returns>
        public static string ZhuanHuaRGB(int yanSe)
        {
            var blue = yanSe / 65536;
            var green = (yanSe - blue * 65536) / 256;
            var red = yanSe - green * 256 - blue * 65536;
            return blue + "," + green + "," + red;
        }

        /// <summary>
        /// 传入一个长字符返回一个RGB型
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color TransferToRGB(this long color)
        {
            int blue = Convert.ToInt32(color / 65536);
            int green = Convert.ToInt32((color - blue * 65536) / 256);
            int red = Convert.ToInt32(color - green * 256 - blue * 65536);
            return Color.FromArgb(red, green, blue);
        }
    }
}
