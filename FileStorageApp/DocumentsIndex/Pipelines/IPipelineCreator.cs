using System;
using Nest;

namespace DocumentsIndex.Pipelines
{
    public interface IPipelineCreator
    {
        Func<PutPipelineDescriptor, PutPipelineDescriptor> CreateElasticDocumentPipeLine();
    }
}