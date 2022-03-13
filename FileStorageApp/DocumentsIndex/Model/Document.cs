using System;
using Nest;

namespace DocumentsIndex.Model
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class Document
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }
}