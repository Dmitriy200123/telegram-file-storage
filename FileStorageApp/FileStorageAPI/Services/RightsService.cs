using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class RightsService : IRightsService
    {
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoStorageFactory"></param>
        public RightsService(IInfoStorageFactory infoStorageFactory)
        {
            _infoStorageFactory = infoStorageFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RequestResult<int[]>> GetUserRights(Guid id)
        {
            using var rightsStorage = _infoStorageFactory.CreateRightsStorage();
            var userRight = await rightsStorage.GetUserRights(id);
            return RequestResult.Ok(userRight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<RequestResult<List<RightDescription>>> GetRightsDescription()
        {
            var descriptions = Enum.GetValues(typeof(Accesses))
                .Cast<Accesses>()
                .Where(x => x != Accesses.Default)
                .Select(t => new RightDescription((int)t, t.GetEnumDescription()))
                .ToList();
            return Task.FromResult(RequestResult.Ok(descriptions));
        }

        public async Task<RequestResult<bool>> UpdateUserRights(RightEdition rightEdition)
        {
            using var rightsStorage = _infoStorageFactory.CreateRightsStorage();
            if(rightEdition.Grant is null && rightEdition.Revoke is null || rightEdition.UserId == null)
                return RequestResult.BadRequest<bool>("Invalid data in body");
            var userId = rightEdition.UserId.Value;
            var contains = await rightsStorage.ContainsAsync(userId);
            if(!contains)
                return RequestResult.NotFound<bool>("No such user");
            if (rightEdition.Revoke != null)
                foreach (var access in rightEdition.Revoke)
                    await rightsStorage.RemoveRight(userId, access);

            if (rightEdition.Grant != null)
                foreach (var access in rightEdition.Grant)
                {
                    var right = new Right
                    {
                        Id = userId,
                        Access = access,
                    };
                    await rightsStorage.AddAsync(right);
                }

            return RequestResult.Ok(true);
        }
    }
}