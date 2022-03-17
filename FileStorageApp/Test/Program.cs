// See https://aka.ms/new-console-template for more information

using System;
using System.Threading;
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
            var text = "YWJvYmlr";
            storage.IndexDocument(guid, text).GetAwaiter().GetResult();
            //storage.DeleteDocument(guid).GetAwaiter().GetResult();
            Thread.Sleep(500);
            var b = storage.GetDoc(text);
            var a = 1;
        }
    }
}