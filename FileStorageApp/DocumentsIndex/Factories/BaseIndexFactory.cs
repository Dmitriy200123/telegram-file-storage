using System;
using System.Text;
using DocumentsIndex.Config;
using Elasticsearch.Net;
using Nest;

namespace DocumentsIndex.Factories
{
    public static class BaseIndexFactory
    {
        public static IDocumentIndexStorage CreateDocumentIndexStorage(ElasticConfig elasticConfig,
            Func<PutPipelineDescriptor, PutPipelineDescriptor> pipelineDescriptor,
            Func<CreateIndexDescriptor, CreateIndexDescriptor> mapping)
        {
            var settings = new ConnectionSettings(new Uri(elasticConfig.Uri))
                .DefaultIndex(elasticConfig.Index)
                .DisableDirectStreaming()
                .PrettyJson()
                .DefaultFieldNameInferrer(p => p)
                .OnRequestCompleted(CreateLogging);

            var elasticClient = new ElasticClient(settings);

            if (elasticClient.Indices.Exists(elasticConfig.Index).Exists)
                elasticClient.Indices.Delete(elasticConfig.Index);

            elasticClient.Indices.Create(elasticConfig.Index, mapping);
            elasticClient.Ingest.PutPipeline(elasticConfig.Index, pipelineDescriptor);
            return new DocumentIndexStorage(elasticClient);
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