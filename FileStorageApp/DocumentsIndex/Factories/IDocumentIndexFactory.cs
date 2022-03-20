using DocumentsIndex.Config;

namespace DocumentsIndex.Factories
{
    public interface IDocumentIndexFactory
    {
        IDocumentIndexStorage CreateDocumentIndexStorage(IElasticConfig elasticConfig);
    }
}