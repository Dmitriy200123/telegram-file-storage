using System;
using DocumentsIndex.Config;
using DocumentsIndex.Model;
using DocumentsIndex.Pipelines;
using Nest;
using Analyzers = DocumentsIndex.Constants.Analyzers;

namespace DocumentsIndex.Factories
{
    /// <summary>
    /// Класс отвечающий за создание эластик клиента
    /// </summary>
    public class DocumentIndexFactory : BaseIndexFactory, IDocumentIndexFactory
    {
        private readonly IPipelineCreator _pipelineCreator;
        private readonly IElasticConfig _elasticConfig;

        public DocumentIndexFactory(IPipelineCreator pipelineCreator, IElasticConfig elasticConfig)
        {
            _pipelineCreator = pipelineCreator ?? throw new ArgumentNullException(nameof(pipelineCreator));
            _elasticConfig = elasticConfig ?? throw new ArgumentNullException(nameof(elasticConfig));
        }

        private static readonly Func<CreateIndexDescriptor, CreateIndexDescriptor> IndexDescriptor = x =>
        {
            return x
                    .Settings(s => s
                        .Analysis(a => a
                            .Analyzers(an => an
                                .Custom(Analyzers.CustomAnalyzer, cu =>
                                    cu.Tokenizer("standard")
                                        .Filters("lowercase", "russian_stop", "russian_keywords", "russian_stemmer", "english_possessive_stemmer",
                                            "english_stop",
                                            "english_stemmer")
                                )
                            )
                            .TokenFilters(tf => tf
                                .KeywordMarker("russian_keywords", km => km
                                    .Keywords()
                                )
                                .Stop("russian_stop", st => st
                                    .StopWords("_russian_"))
                                .Stemmer("russian_stemmer", st => st
                                    .Language("russian"))
                                .Stemmer("english_stemmer", st => st
                                    .Language("english"))
                                .Stemmer("english_possessive_stemmer", st => st
                                    .Language("possessive_english"))
                                .Stop("english_stop", st => st
                                    .StopWords("_english_"))
                            )
                        ))
                    .Map<ElasticDocument>(mp => mp
                        .AutoMap()
                        .Properties(ps => ps
                            .Keyword(k => k
                                .Name(n => n.Id))
                            .Text(t => t
                                .Name(n => n.Content)
                                .Analyzer(Analyzers.CustomAnalyzer))
                            .Text(t => t
                                .Name(n => n.Name)
                                .Analyzer(Analyzers.CustomAnalyzer))
                            .Object<Attachment>(a => a
                                .Name(n => n.Attachment)
                                .AutoMap()
                                .Properties(pp => pp
                                    .Text(t => t
                                        .Name(n => n.Content)
                                        .Analyzer(Analyzers.CustomAnalyzer)))
                            )
                        )
                    )
                ;
        };

        /// <inheritdoc />
        public IDocumentIndexStorage CreateDocumentIndexStorage()
        {
            var pipeline = _pipelineCreator.CreateElasticDocumentPipeLine();
            var storage = CreateDocumentIndexStorage(_elasticConfig, pipeline, IndexDescriptor);
            return storage;
        }
    }
}