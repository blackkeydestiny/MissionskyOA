using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MissionskyOA.Core.Enum;

namespace System
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 是否为有效的申请类型
        /// </summary>
        /// <param name="input">整数值</param>
        /// <returns>true or false</returns>
        public static bool IsValidOrderType(this int input)
        {
            Array values = Enum.GetValues(typeof (OrderType));

            if (input == (int)OrderType.None)
            {
                return false;
            }

            string temp = "";
            foreach (int v in values)
            {
                temp += "," + v;
            }

            temp += ",";

            return temp.Contains(string.Format(",{0},", input));
        }
        
        /// <summary>
        /// 获取枚举的描述信息
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns>枚举的描述信息集合</returns>
        public static NameValueCollection GetDescriptionList(Type type)
        {
            NameValueCollection collection = new NameValueCollection();
            Type attrType = typeof (DescriptionAttribute);

            foreach (FieldInfo field in type.GetFields())
            {
                object[] objs = field.GetCustomAttributes(attrType, true);

                if (objs.Length > 0)
                {
                    collection.Add(field.Name, ((DescriptionAttribute) objs[0]).Description);
                }
            }

            return collection;
        }
        
        /// <summary>
        /// 获取枚举
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns>枚举的描述信息集合</returns>
        public static ArrayList GetEmnuValueList(Type type)
        {
            ArrayList values = new ArrayList();

            foreach (FieldInfo field in type.GetFields())
            {
                if (!field.Name.ToLower().Equals("value__"))
                {
                    values.Add(field.Name);
                }
            }

            return values;
        }
    }
}
