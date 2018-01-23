using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static bool IsMobileWithHK(this string input)
        {
            string pattern = "^[5,6,9]\\d{7}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        public static bool IsEmail(this string input)
        {
            string pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        public static bool IsNotMobileOrEmail(this string input)
        {
            return !IsEmail(input) && !IsMobileWithHK(input);
        }

        /// <summary>
        /// string.IsNullOrWhitespace方法的简写
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <returns>如果字符串不为空白则返回true</returns>
        public static bool IsNotBlank(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 去掉字符串开头结尾的空白，相当于Trim。
        /// 但对于参数为null的情况则返回空白字符串，以避免出现空指针异常。
        /// </summary>
        /// <param name="str">要截取的字符串</param>
        /// <returns>截取结果。该方法保证结果不为null</returns>
        public static string SafeTrim(this string str)
        {
            return (str != null) ? str.Trim() : string.Empty;
        }

        /// <summary>
        /// 用忽略大小写的方式检查两个字符串是否相等。
        /// </summary>
        /// <param name="str">比较左值</param>
        /// <param name="other">比较右值</param>
        /// <returns>两个字符串是否相等的结果，忽略大小写</returns>
        public static bool EqualsIgnoreCase(this string str, string other)
        {
            return string.Compare(str, other, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// 检查字符串是否包含指定的字串，并忽略大小写
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <param name="substr">子串</param>
        /// <returns>如果包含则返回true，否则返回false。null视为不包含</returns>
        public static bool ContainsIgnoreCase(this string str, string substr)
        {
            if (str == null || substr == null)
                return false;
            return str.ToLower().Contains(substr.ToLower());
        }
    }
}
