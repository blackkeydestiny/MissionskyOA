using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class IntegerExtensions
    {
        /// <summary>
        /// 小数四舍五入到整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Round(this double value)
        {
            int inte = (int)Math.Floor(value); //整数部分
            double dec = value - inte; //小数部分

            return (dec >= 0.5 ? inte + 1 : inte);
        }

        /// <summary>
        /// 小数四舍五入到整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Round(this decimal value)
        {
            int inte = (int)Math.Floor(value); //整数部分
            decimal dec = value - inte; //小数部分

            return (dec >= (decimal)0.5 ? inte + 1 : inte);
        }

        /// <summary>
        /// 判定公历闰年遵循的一般规律为：四年一闰，百年不闰，四百年再闰。
        /// 公历闰年的精确计算方法：（按一回归年365天5小时48分45.5秒）
        /// 普通年能被4整除而不能被100整除的为闰年。 （如2004年就是闰年，1900年不是闰年）
        /// 世纪年能被400整除而不能被3200整除的为闰年。 (如2000年是闰年，3200年不是闰年)
        /// 对于数值很大的年份能整除3200,但同时又能整除172800则又是闰年。(如172800年是闰年，86400年不是闰年）
        /// 
        /// 公元前闰年规则如下：
        /// 非整百年：年数除4余数为1是闰年，即公元前1、5、9……年；
        /// 整百年：年数除400余数为1是闰年，年数除3200余数为1，不是闰年,年数除172800余1又为闰年，即公元前401、801……年。
        /// </summary>
        /// <param name="yN">年份数字</param>
        /// <returns></returns>
        public static bool IsLeap(this int year)
        {

            if ((year % 400 == 0 && year % 3200 != 0)
               || (year % 4 == 0 && year % 100 != 0)
               || (year % 3200 == 0 && year % 172800 == 0))
                return true;
            else
                return false;
        }
    }
}
