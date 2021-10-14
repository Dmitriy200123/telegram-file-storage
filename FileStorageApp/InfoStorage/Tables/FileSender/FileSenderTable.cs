using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Tables.FileSender
{
    internal class FileSenderTable : IFileSenderTable
    {
        private IMainStorage _mainStorage;

        public FileSenderTable(IMainStorage mainStorage)
        {
            _mainStorage = mainStorage;
        }

        public Models.FileSender GetFileSenderById(Guid id)
        {
            var fileSender = _mainStorage.FileSenders.FirstOrDefault(x => x.Id == id);
            if (fileSender == null)
                throw new ArgumentException($"User with id {id} not found");
            return fileSender;
        }
    }
}