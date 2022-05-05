using API;

namespace FileStorageAPI.Extensions
{
    /// <summary>
    /// Функции расширения для RequestResult
    /// </summary>
    public static class RequestResultExtension
    {
        /// <summary>
        /// Преобразует RequestResult в NotFoundResult
        /// </summary>
        /// <typeparam name="T">Параметр RequestResult</typeparam>
        public static NotFoundResult ToNotFoundResult<T>(this RequestResult<T> result)
        {
            if (result.ExtraOptions == null)
                return new NotFoundResult { Message = result.Message };
            
            var entityName = result.ExtraOptions.ContainsKey(ExtraOption.EntityName)
                ? result.ExtraOptions[ExtraOption.EntityName]
                : null;

            return new NotFoundResult { Message = result.Message, EntityName = entityName };
        }
    }
}