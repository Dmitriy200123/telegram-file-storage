namespace DocumentsIndex.Factories
{
    public interface IDocumentIndexFactory
    {
        IDocumentIndexStorage CreateDocumentIndexStorage();
    }
}