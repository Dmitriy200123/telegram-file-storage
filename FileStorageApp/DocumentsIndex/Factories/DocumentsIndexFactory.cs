using System;
using System.Text;
using DocumentsIndex.Config;
using Elasticsearch.Net;
using Nest;

namespace DocumentsIndex.Factories
{
    public class DocumentsIndexFactory : IDocumentsIndexFactory
    {
        private readonly IElasticClient _elasticClient;
        private const string DefaultIndex = "index";

        public DocumentsIndexFactory(IElasticConfig elasticConfig,
            Func<PutPipelineDescriptor, PutPipelineDescriptor> pipelineDescriptor,
            Func<CreateIndexDescriptor, CreateIndexDescriptor> mapping)
        {
            var settings = new ConnectionSettings(new Uri(elasticConfig.Uri))
                .DefaultIndex(DefaultIndex)
                .DisableDirectStreaming()
                .PrettyJson()
                .DefaultFieldNameInferrer(p => p)
                .OnRequestCompleted(CreateLogging);

            _elasticClient = new ElasticClient(settings);

            if (_elasticClient.Indices.Exists(elasticConfig.Index).Exists)
                _elasticClient.Indices.Delete(elasticConfig.Index);

            _elasticClient.Indices.Create(elasticConfig.Index, mapping);
            _elasticClient.Ingest.PutPipeline(elasticConfig.Index, pipelineDescriptor);
        }

        public IDocumentIndexStorage CreateDocumentIndexStorage()
        {
            return new DocumentIndexStorage(_elasticClient);
        }

        private static void CreateLogging(IApiCallDetails callDetails)
        {
            {
                if (callDetails.RequestBodyInBytes != null)
                {
                    Console.WriteLine(
                        $"{callDetails.HttpMethod} {callDetails.Uri} \n" +
                        $"{Encoding.UTF8.GetString(callDetails.RequestBodyInBytes)}");
                }
                else
                {
                    Console.WriteLine($"{callDetails.HttpMethod} {callDetails.Uri}");
                }

                Console.WriteLine();

                if (callDetails.ResponseBodyInBytes != null)
                {
                    Console.WriteLine($"Status: {callDetails.HttpStatusCode}\n" +
                                      $"{Encoding.UTF8.GetString(callDetails.ResponseBodyInBytes)}\n" +
                                      $"{new string('-', 30)}\n");
                }
                else
                {
                    Console.WriteLine($"Status: {callDetails.HttpStatusCode}\n" +
                                      $"{new string('-', 30)}\n");
                }
            }
        }
    }
}