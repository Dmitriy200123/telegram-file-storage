using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace FileStorageApp.Data.InfoStorage.Storages
{
    public interface IInfoStorage<T>
    {
        public Task<bool> AddAsync(T entity, bool writeException = true);

        public Task<bool> UpdateAsync(T entity);

        public Task<bool> DeleteAsync(Guid id);

        public Task<bool> ContainsAsync(Guid id);
        public DatabaseFacade Database { get; }
    }
}