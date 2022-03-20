using System;

namespace DocumentsIndex.Model
{
    public class Document
    {
        public byte[] Content { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Document(Guid id, byte[] content, string name)
        {
            Id = id;
            Content = content;
            Name = name;
        }
    }
}