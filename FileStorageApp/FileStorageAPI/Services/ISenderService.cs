using System;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    public interface ISenderService
    {
        Task<Sender> GetSenderById(Guid id);
        Task<Sender> CreateSender(Sender sender);
        Task<Sender> UpdateSender(Guid id, Sender sender);
    }
}