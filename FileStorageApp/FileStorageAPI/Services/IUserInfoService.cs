using System;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    public interface IUserInfoService
    {
        Task<RequestResult<UserInfo>> GetUserInfo(Guid id);
    }
}