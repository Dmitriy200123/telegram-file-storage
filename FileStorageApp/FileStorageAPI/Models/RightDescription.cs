namespace FileStorageAPI.Models
{
    /// <summary>
    /// Класс отвечающий за сопоставление названия права и его идентификатора
    /// </summary>
    public class RightDescription
    {
        /// <summary>
        /// Конструктор в котором задаются
        /// </summary>
        /// <param name="id">Идентификатор права</param>
        /// <param name="name">Название права</param>
        public RightDescription(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Название права
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Идентификатор права
        /// </summary>
        public int Id { get; set; }
    }
}