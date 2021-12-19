using System;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public interface IModel
    {
        public Guid UserId { get; set; }
    }
}