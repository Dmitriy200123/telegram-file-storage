using System;
using Nest;

namespace DocumentsIndex.Model
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }
}