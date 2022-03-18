// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Linq;
using System.Threading;
using DocumentsIndex;
using DocumentsIndex.Config;
using DocumentsIndex.Factories;
using DocumentsIndex.Model;
using Nest;

namespace Test
{
    public class Test
    {
        public static void Main(string[] args)
        {
            var defaultIndex = "attachments";
            var pipelineDescriptor = new Func<PutPipelineDescriptor, PutPipelineDescriptor>(x =>
            {
                return x.Description("Document attachment pipeline")
                    .Processors(pr => pr
                        .Attachment<ElasticDocument>(a => a
                            .Field(f => f.Content)
                            .TargetField(f => f.Attachment)
                        )
                        .Remove<ElasticDocument>(r => r
                            .Field(ff => ff
                                .Field(f => f.Content)
                            )
                        )
                    );
            });
            var m = new Func<CreateIndexDescriptor, CreateIndexDescriptor>(x =>
            {
                return x.Settings(s => s
                        .Analysis(a => a
                            .Analyzers(ad => ad
                                .Custom("windows_path_hierarchy_analyzer", ca => ca
                                    .Tokenizer("windows_path_hierarchy_tokenizer")
                                )
                            )
                            .Tokenizers(t => t
                                .PathHierarchy("windows_path_hierarchy_tokenizer", ph => ph
                                    .Delimiter('\\')
                                )
                            )
                        )
                    )
                    .Map<ElasticDocument>(mp => mp
                        .AutoMap()
                        .Properties(ps => ps
                            .Keyword(k => k
                                .Name(n => n.Id))
                            .Text(t => t
                                .Name(n => n.Content))
                            .Object<Attachment>(a => a
                                .Name(n => n.Attachment)
                                .AutoMap()
                            )
                        )
                    );
            });

            var bytes = File.ReadAllBytesAsync(@"C:\elastic\example_one.docx").GetAwaiter().GetResult();
            var elasticConfig = new ElasticConfig("http://localhost:9200", defaultIndex);
            var factory = new DocumentsIndexFactory(elasticConfig, pipelineDescriptor, m);
            var storage = factory.CreateDocumentIndexStorage();
            var guid = Guid.NewGuid();
            storage.IndexDocument(bytes, guid).GetAwaiter().GetResult();

            var searchResponse = storage.Search("NEST").GetAwaiter().GetResult();


            foreach (var hit in searchResponse.Hits)
            {
                var attachment = hit.Source.Attachment;
                Console.WriteLine($"Attachment details for doc id {hit.Id}");
                Console.WriteLine($"date: {attachment.Date}");
                Console.WriteLine($"content type: {attachment.ContentType}");
                Console.WriteLine($"author: {attachment.Author}");
                Console.WriteLine($"language: {attachment.Language}");
                Console.WriteLine($"content: {attachment.Content}");
                Console.WriteLine($"content length: {attachment.ContentLength}");
            }

            var count = searchResponse.Hits
                .Select(x => storage.Delete(x.Source.Id).GetAwaiter().GetResult())
                .Count(x => !x);
            Console.WriteLine(count);
        }
    }
}