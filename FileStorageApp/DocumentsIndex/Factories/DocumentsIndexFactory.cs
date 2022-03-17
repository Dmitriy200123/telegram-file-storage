﻿using System;
using System.Collections.Generic;
using System.Text;
using DocumentsIndex.Config;
using DocumentsIndex.Model;
using Elasticsearch.Net;
using Nest;

namespace DocumentsIndex.Factories
{
    public class DocumentsIndexFactory : IDocumentsIndexFactory
    {
        private readonly IElasticClient _elasticClient;

        public DocumentsIndexFactory(IElasticConfig elasticConfig)
        {
            var settings = new ConnectionSettings(new Uri(elasticConfig.Uri))
                .DisableDirectStreaming()
                .DefaultIndex("index")
                .DefaultFieldNameInferrer(p => p)
                .PrettyJson()
                .OnRequestCompleted(CreateLogging);
            _elasticClient = new ElasticClient(settings);
            CreateMapping();
        }

        public IDocumentIndexStorage CreateDocumentIndexStorage()
        {
            return new DocumentIndexStorage(_elasticClient);
        }

        private void CreateMapping()
        {
            _elasticClient.Indices.Create("document", c => c
                .Map<ElasticDocument>(m => m
                    .Properties(ps => ps
                        .Text(s => s
                            .Name(n => n.Text)
                        )
                        .Keyword(k =>
                            k.Name(n => n.Id))
                    )
                )
            );
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