using System;
using DocumentsIndex.Config;
using DocumentsIndex.Model;
using Nest;

namespace DocumentsIndex.Factories
{
    public static class DocumentIndexFactory
    {
        private static readonly Func<PutPipelineDescriptor, PutPipelineDescriptor> PipeLineDescriptor = x =>
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
        };
        
        private static readonly Func<CreateIndexDescriptor, CreateIndexDescriptor> IndexDescriptor = x =>
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
        };

        public static IDocumentIndexStorage CreateDocumentIndexStorage(ElasticConfig elasticConfig)
        {
            var storage = BaseIndexFactory.CreateDocumentIndexStorage(elasticConfig, PipeLineDescriptor, IndexDescriptor);
            return storage;
        }
    }
}