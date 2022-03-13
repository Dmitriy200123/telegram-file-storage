// See https://aka.ms/new-console-template for more information

using System;
using DocumentsIndex.Config;
using DocumentsIndex.Factories;

namespace Test
{
    public class Test
    {
        public static void Main(string[] args)
        {
            var elasticConfig = new ElasticConfig("http://localhost:9200");
            var factory = new DocumentsIndexFactory(elasticConfig);
            var storage = factory.CreateDocumentIndexStorage();
            var guid = Guid.NewGuid();
            storage.IndexDocument(guid, "YWJvYmlr").GetAwaiter().GetResult();
            storage.DeleteDocument(guid).GetAwaiter().GetResult();

        }
    }
}