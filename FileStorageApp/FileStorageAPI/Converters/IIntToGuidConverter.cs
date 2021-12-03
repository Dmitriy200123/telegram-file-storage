using System;

namespace FileStorageAPI.Converters
{
    /// <summary>
    /// Класс, который конвертирует int в Guid
    /// </summary>
    public interface IIntToGuidConverter
    {
        /// <summary>
        /// Метод конвертации int в Guid
        /// </summary>
        /// <param name="value">значение типа int</param>
        /// <returns></returns>
        Guid Convert(int value);
    }
}