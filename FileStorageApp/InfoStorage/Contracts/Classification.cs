using System;
using System.Collections.Generic;

namespace FileStorageApp.Data.InfoStorage.Contracts
{
    public class Classification
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public ICollection<ClassificationWord> ClassificationWords { get; set; } = new List<ClassificationWord>();
    }
}