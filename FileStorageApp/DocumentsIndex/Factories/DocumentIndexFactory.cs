using System;
using System.Collections.Generic;
using DocumentsIndex.Config;
using DocumentsIndex.Model;
using DocumentsIndex.Pipelines;
using Nest;
using Analyzers = DocumentsIndex.Constants.Analyzers;
using Tokenizers = DocumentsIndex.Constants.Tokenizers;

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
            var nGramFilters = new List<string> {"lowercase", "asciifolding"};
            return x.Settings(st => st
                        .Setting(UpdatableIndexSettings.MaxNGramDiff, 17)
                        .Analysis(an => an
                            .Analyzers(anz => anz
                                .Custom(Analyzers.DocumentNgramAnalyzer, cc => cc
                                    .Tokenizer(Tokenizers.DocumentNgramTokenizer)
                                    .Filters(nGramFilters))
                            )
                            .Tokenizers(tz => tz
                                .EdgeNGram(Tokenizers.DocumentNgramTokenizer, td => td
                                    .MinGram(3)
                                    .MaxGram(20)
                                    .TokenChars(
                                        TokenChar.Letter,
                                        TokenChar.Digit,
                                        TokenChar.Punctuation,
                                        TokenChar.Symbol
                                    )
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
                            .Text(t => t
                                .Name(n => n.Name)
                                .Analyzer(Analyzers.DocumentNgramAnalyzer))
                            .Object<Attachment>(a => a
                                .Name(n => n.Attachment)
                                .AutoMap()
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