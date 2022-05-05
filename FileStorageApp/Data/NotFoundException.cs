using System;

namespace Data
{
    public class NotFoundException : Exception
    {
        public readonly string EntityName;

        public NotFoundException(string message, string entityName = default) : base(message)
        {
            EntityName = entityName;
        }

        public static NotFoundException NotFoundEntity<T>(string message) => new(message, nameof(T));
    }
}