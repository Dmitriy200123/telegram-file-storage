using System;
using Nest;

namespace DocumentsIndex.Pipelines
{
    /// <summary>
    /// Создатель пайплайна для эластика
    /// </summary>
    public interface IPipelineCreator
    {
        /// <summary>
        /// Функция отвечающая за создание пайплайна
        /// </summary>
        /// <returns></returns>
        Func<PutPipelineDescriptor, PutPipelineDescriptor> CreateElasticDocumentPipeLine();
    }
}