using System;
using DocumentsIndex.Model;
using Nest;

namespace DocumentsIndex.Pipelines
{
    public class PipelineCreator : IPipelineCreator
    {
        public Func<PutPipelineDescriptor, PutPipelineDescriptor> CreateElasticDocumentPipeLine()
        {
            return x => x
                .Description("Document attachment pipeline")
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
        }
    }
}