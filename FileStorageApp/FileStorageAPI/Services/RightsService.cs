using System;
using System.Linq;
using System.Threading.Tasks;
using API;
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
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _enumValues = Enum.GetValues(typeof(Accesses)).Cast<Accesses>().Cast<int>().ToArray();
        }

        /// <inheritdoc />
        public async Task<RequestResult<int[]>> GetCurrentUserRights(Guid userId)
        {
            using var rightsStorage = _infoStorageFactory.CreateRightsStorage();
            var userRight = await rightsStorage.GetUserRightsAsync(userId);
            return RequestResult.Ok(userRight);
        }

        /// <inheritdoc />
        public async Task<RequestResult<int[]>> GetUserRights(Guid id)
        {
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var user = await usersStorage.GetByIdAsync(id, true);
            return user is null
                ? RequestResult.NotFound<int[]>("Не найден пользователь с данным идентификатором")
                : RequestResult.Ok(user.Rights.Select(right => right.Access).ToArray());
        }

        /// <inheritdoc />
        public RequestResult<RightDescription[]> GetRightsDescription()
        {
            var descriptions = Enum.GetValues(typeof(Accesses))
                .Cast<Accesses>()
                .Where(x => x != Accesses.Default)
                .Select(t => new RightDescription((int) t, t.GetEnumDescription()))
                .ToArray();

            return RequestResult.Ok(descriptions);
        }

        /// <inheritdoc />
        public async Task<RequestResult<bool>> UpdateUserRights(RightEdition rightEdition)
        {
            if (rightEdition.Grant is null && rightEdition.Revoke is null)
                return RequestResult.BadRequest<bool>("Invalid data in body");
            using var rightsStorage = _infoStorageFactory.CreateRightsStorage();
            var userId = rightEdition.UserId;
            var contains = await rightsStorage.ContainsAsync(userId);
            if (!contains)
                return RequestResult.NotFound<bool>("No such user");

            if (IsContainsBadIds(rightEdition.Revoke) || IsContainsBadIds(rightEdition.Grant))
                return RequestResult.BadRequest<bool>("Invalid rights Ids");

            if (rightEdition.Revoke != null)
                foreach (var access in rightEdition.Revoke)
                    await rightsStorage.RemoveRightAsync(userId, access);

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