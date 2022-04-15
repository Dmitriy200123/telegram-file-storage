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
        /// <returns></returns>
        IDocumentIndexStorage CreateDocumentIndexStorage();
    }
}