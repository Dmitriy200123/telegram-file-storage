using System;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;

namespace FileStorageAPI.Services
{
    class UserInfoService : IUserInfoService
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IUserConverter _userConverter;

        public UserInfoService(IInfoStorageFactory infoStorageFactory, IUserConverter userConverter)
        {
            _infoStorageFactory = infoStorageFactory;
            _userConverter = userConverter;
        }

        public async Task<RequestResult<UserInfo>> GetUserInfo(Guid id)
        {
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var user = await usersStorage.GetByIdAsync(id);
            return user is null 
                ? RequestResult.NotFound<UserInfo>("No such user in database") 
                : RequestResult.Ok(_userConverter.ConvertUser(user));
        }
    }
}