using Nest;

namespace DocumentsIndex.Factories
{
    public interface IBaseIndexFactory
    {
        IDocumentIndexStorage CreateDocumentIndexStorage();
    }
}