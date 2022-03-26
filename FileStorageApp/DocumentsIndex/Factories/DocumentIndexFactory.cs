﻿using System;
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

        public DocumentIndexFactory(IPipelineCreator pipelineCreator)
        {
            _pipelineCreator = pipelineCreator ?? throw new ArgumentNullException(nameof(pipelineCreator));
        }

        private static readonly Func<CreateIndexDescriptor, CreateIndexDescriptor> IndexDescriptor = x =>
        {
            var nGramFilters = new List<string> {"lowercase", "asciifolding"};
            return x.Settings(st => st
                        .Setting(UpdatableIndexSettings.MaxNGramDiff, 18)
                        .Analysis(an => an
                            .Analyzers(anz => anz
                                .Custom(Analyzers.DocumentNgramAnalyzer, cc => cc
                                    .Tokenizer(Tokenizers.DocumentNgramTokenizer)
                                    .Filters(nGramFilters))
                            )
                            .Tokenizers(tz => tz
                                .NGram(Tokenizers.DocumentNgramTokenizer, td => td
                                    .MinGram(2)
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
        public IDocumentIndexStorage CreateDocumentIndexStorage(IElasticConfig elasticConfig)
        {
            var pipeline = _pipelineCreator.CreateElasticDocumentPipeLine();
            var storage = CreateDocumentIndexStorage(elasticConfig, pipeline, IndexDescriptor);
            return storage;
        }
    }
}