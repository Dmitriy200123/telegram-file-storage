using System;
using System.ComponentModel;

namespace FileStorageAPI.Extensions
{
    /// <summary>
    /// Методы расширения для перечислений
    /// </summary>
    public static class EnumsExtension
    {
        /// <summary>
        /// Возвращает значение атрибута Description 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi?.GetCustomAttributes(typeof(DescriptionAttribute), false)!;

            return attributes.Length > 0 
                ? attributes[0].Description 
                : value.ToString();
        }
    }
}