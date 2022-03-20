using System;
using Nest;

namespace DocumentsIndex.Model
{
    internal class ElasticDocument
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public Attachment Attachment { get; set; }
    }
}