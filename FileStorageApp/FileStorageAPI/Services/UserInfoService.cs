using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    class UserInfoService : IUserInfoService
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IUserConverter _userConverter;

        public UserInfoService(IInfoStorageFactory infoStorageFactory, IUserConverter userConverter)
        {
            _infoStorageFactory = infoStorageFactory;
            _userConverter = userConverter;
        }
        /// <inheritdoc />
        public async Task<RequestResult<UserInfo>> GetUserInfo(Guid id)
        {
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var user = await usersStorage.GetByIdAsync(id);
            return user is null 
                ? RequestResult.NotFound<UserInfo>("No such user in database") 
                : RequestResult.Ok(_userConverter.ConvertUser(user));
        }
        /// <inheritdoc />
        public async Task<RequestResult<List<UserIdAndFio>>> GetUsersInfo()
        {
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var users = await usersStorage.GetAll();
            var convertedUsers = users.Select(_userConverter.ConvertUserToIdAndFio).ToList();
            return RequestResult.Ok(convertedUsers);
        }
    }
}