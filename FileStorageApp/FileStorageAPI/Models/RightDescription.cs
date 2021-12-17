namespace FileStorageAPI.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RightDescription
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public RightDescription(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
    }
}