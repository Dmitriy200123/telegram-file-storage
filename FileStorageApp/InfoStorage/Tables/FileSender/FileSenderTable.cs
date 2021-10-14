using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Tables.FileSender
{
    public class FileSenderTable : IFileSenderTable
    {
        private DbSet<Models.FileSender> _fileSenders { get; set; }

        internal FileSenderTable(IMainStorage mainStorage)
        {
            _fileSenders = mainStorage.FileSenders;
        }

        public Models.FileSender GetFileSenderById(Guid id)
        {
            var fileSender = _fileSenders.FirstOrDefault(x => x.Id == id);
            if (fileSender == null)
                throw new ArgumentException($"User with id {id} not found");
            return fileSender;
        }
    }
}