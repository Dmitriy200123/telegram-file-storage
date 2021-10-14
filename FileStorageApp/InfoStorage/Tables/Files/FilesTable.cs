using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Tables.Files
{
    public class FilesTable : IFilesTable
    {
        private DbSet<Models.File> _files { get; set; }
        internal FilesTable(IMainStorage mainStorage)
        {
            _files = mainStorage.Files;
        }
        public Models.File GetFileById(Guid id)
        {
            var fileSender = _files.FirstOrDefault(x => x.Id == id);
            if (fileSender == null)
                throw new ArgumentException($"File with id {id} not found");
            return fileSender;
        }
    }
}