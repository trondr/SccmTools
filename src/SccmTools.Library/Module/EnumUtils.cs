using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SccmTools.Library.Module
{
    public class EnumUtils
    {
        public static string StringValueOf(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            throw new ArgumentException("The value do not have a a description value.");
        }

        public static object EnumValueOf(string value, Type enumType)
        {
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                if (StringValueOf((Enum)Enum.Parse(enumType, name)).Equals(value))
                {
                    return Enum.Parse(enumType, name);
                }
            }
            throw new ArgumentException("The string is not a description or value of the specified enum.");
        }

        /// <summary>
        /// EnumToList() takes an enum type and returns a generic list populated with each enum item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> EnumValueToList<T>() where T : struct
        {
            Type enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }
            // ReSharper disable AssignNullToNotNullAttribute
            return new List<T>(Enum.GetValues(enumType) as IEnumerable<T>);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>  Gets the enum string value to list. </summary>
        ///
        /// <remarks>  Eta 410, 05.06.2012. </remarks>
        ///
        /// <typeparam name="T">   Generic type parameter. </typeparam>
        ///
        /// <returns>  . </returns>

        public static List<string> EnumStringValueToList<T>() where T : struct
        {
            Type enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }
            List<T> enumValues = EnumValueToList<T>();
            return enumValues.Select(enumValue => StringValueOf(enumValue as Enum)).ToList();
        }
    }
}