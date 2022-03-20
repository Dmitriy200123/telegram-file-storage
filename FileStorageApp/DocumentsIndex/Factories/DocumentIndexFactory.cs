using System;
using DocumentsIndex.Config;
using DocumentsIndex.Model;
using DocumentsIndex.Pipelines;
using Nest;

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
            _pipelineCreator = pipelineCreator ?? throw new ArgumentNullException();
        }

        private static readonly Func<CreateIndexDescriptor, CreateIndexDescriptor> IndexDescriptor = x =>
        {
            return x
                .Map<ElasticDocument>(mp => mp
                    .AutoMap()
                    .Properties(ps => ps
                        .Keyword(k => k
                            .Name(n => n.Id))
                        .Text(t => t
                            .Name(n => n.Content))
                        .Text(t => t
                            .Name(n => n.Name))
                        .Object<Attachment>(a => a
                            .Name(n => n.Attachment)
                            .AutoMap()
                        )
                    )
                );
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