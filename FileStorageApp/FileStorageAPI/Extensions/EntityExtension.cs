using System;

namespace FileStorageAPI.Extensions
{
    /// <summary>
    /// Методы расширения для классов
    /// </summary>
    public static class EntityExtension
    {
        /// <summary>
        /// Проверка сущности на null
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <typeparam name="T">Типа сущности</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Ошибка которая выбрасывается если сущность null</exception>
        public static T CheckForNull<T>(this T entity) where T : class?
        {
            if (entity == null)
                throw new InvalidOperationException($"{nameof(T)} was null");
            return entity;
        }
    }
}