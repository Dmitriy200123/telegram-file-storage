using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRightsService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        Task<RequestResult<int[]>> GetUserRights(Guid id);
        
        /// <summary>
        /// 
        /// </summary>
        Task<RequestResult<List<RightDescription>>> GetRightsDescription();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<RequestResult<bool>> UpdateUserRights(RightEdition rightEdition);
    }
}