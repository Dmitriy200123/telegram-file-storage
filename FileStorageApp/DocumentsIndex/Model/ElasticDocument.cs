using System;
using Nest;

namespace DocumentsIndex.Model
{
    public class ElasticDocument
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }
}