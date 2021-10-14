using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Tables.Files
{
    internal class FilesTable : IFilesTable
    {
        private IMainStorage _mainStorage;
        public FilesTable(IMainStorage mainStorage)
        {
            _mainStorage = mainStorage;
        }
        public Models.File GetFileById(Guid id)
        {
            var fileSender = _mainStorage.Files.FirstOrDefault(x => x.Id == id);
            if (fileSender == null)
                throw new ArgumentException($"File with id {id} not found");
            return fileSender;
        }
    }
}