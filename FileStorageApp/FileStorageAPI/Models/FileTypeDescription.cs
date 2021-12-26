namespace FileStorageAPI.Models
{
    /// <summary>
    /// Тип файла с описанием.
    /// </summary>
    public class FileTypeDescription
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название типа файла.
        /// </summary>
        public string Name { get; set; }
    }
}