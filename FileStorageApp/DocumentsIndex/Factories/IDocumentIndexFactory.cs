using DocumentsIndex.Config;

namespace DocumentsIndex.Factories
{
    /// <summary>
    /// Фабрика создающий объект для взаимодействия с хранилищем
    /// </summary>
    public interface IDocumentIndexFactory
    {
        /// <summary>
        /// Метод создающий объект для взаимодействия с хранилищем
        /// </summary>
        /// <param name="elasticConfig">Конфигурация для создания клиента эластика</param>
        /// <returns></returns>
        IDocumentIndexStorage CreateDocumentIndexStorage(IElasticConfig elasticConfig);
    }
}