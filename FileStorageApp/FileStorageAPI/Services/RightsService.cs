using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageAPI.Extensions;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class RightsService : IRightsService
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly int[] _enumValues;

        /// <summary>
        /// Конструктор сервиса управления правами пользователя
        /// </summary>
        /// <param name="infoStorageFactory">Фабрика базы данных</param>
        public RightsService(IInfoStorageFactory infoStorageFactory)
        {
            _infoStorageFactory = infoStorageFactory;
            _enumValues = Enum.GetValues(typeof(Accesses)).Cast<Accesses>().Cast<int>().ToArray();
        }


        /// <inheritdoc />
        public async Task<RequestResult<int[]>> GetUserRights(Guid id)
        {
            using var rightsStorage = _infoStorageFactory.CreateRightsStorage();
            var userRight = await rightsStorage.GetUserRights(id);
            return RequestResult.Ok(userRight);
        }

        /// <inheritdoc />
        public Task<RequestResult<List<RightDescription>>> GetRightsDescription()
        {
            var descriptions = Enum.GetValues(typeof(Accesses))
                .Cast<Accesses>()
                .Where(x => x != Accesses.Default)
                .Select(t => new RightDescription((int) t, t.GetEnumDescription()))
                .ToList();
            return Task.FromResult(RequestResult.Ok(descriptions));
        }

        /// <inheritdoc />
        public async Task<RequestResult<bool>> UpdateUserRights(RightEdition rightEdition)
        {
            using var rightsStorage = _infoStorageFactory.CreateRightsStorage();
            if (rightEdition.Grant is null && rightEdition.Revoke is null)
                return RequestResult.BadRequest<bool>("Invalid data in body");
            var userId = rightEdition.UserId;
            var contains = await rightsStorage.ContainsAsync(userId);
            if (!contains)
                return RequestResult.NotFound<bool>("No such user");
            
            if (IsContainsBadIds(rightEdition.Revoke) || IsContainsBadIds(rightEdition.Grant))
                return RequestResult.BadRequest<bool>("Invalid rights Ids");
            
            if (rightEdition.Revoke != null)
                foreach (var access in rightEdition.Revoke)
                    await rightsStorage.RemoveRight(userId, access);

            if (rightEdition.Grant != null)
                foreach (var access in rightEdition.Grant)
                {
                    var right = new Right
                    {
                        UserId = userId,
                        Access = access,
                    };
                    await rightsStorage.AddAsync(right);
                }

            return RequestResult.Ok(true);
        }

        private bool IsContainsBadIds(int[]? ids)
        {
            return ids != null && ids.Intersect(_enumValues).Count() != ids.Length;
        }
    }
}