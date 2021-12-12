using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.Chats
{
    internal class ChatStorage : BaseStorage<Chat>, IChatStorage
    {
        internal ChatStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
        }

        public Task<List<Chat>> GetAllAsync()
        {
            return DbSet
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public new Task<Chat> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        public Task<List<Chat>> GetByChatNameSubstringAsync(string subString)
        {
            if (subString == null)
                throw new ArgumentNullException(nameof(subString));

            return DbSet
                .Where(x => x.Name.Contains(subString))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<bool> ContainsByTelegramIdAsync(long id)
        {
            return DbSet.AnyAsync(chat => chat.TelegramId == id);
        }
    }
}