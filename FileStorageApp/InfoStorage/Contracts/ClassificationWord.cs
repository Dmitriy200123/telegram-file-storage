using System;

namespace FileStorageApp.Data.InfoStorage.Contracts
{
    public class ClassificationWord
    {
        public Guid Id { get; set; }
        
        public string Value { get; set; }
        
        public Guid ClassificationId { get; set; }
    }
}