namespace DocumentsIndex.Config
{
    /// <summary>
    /// Конфигурация эластика
    /// </summary>
    public interface IElasticConfig
    {
        /// <summary>
        /// Uri по которому можно подключиться к эластику, например http://localhost:9200
        /// </summary>
        string Uri { get; }
        
        /// <summary>
        /// Название индекса по которому будет происходить индексация
        /// </summary>
        string Index { get; }
    }
}