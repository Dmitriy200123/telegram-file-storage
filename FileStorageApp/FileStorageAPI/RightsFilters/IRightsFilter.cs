using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FileStorageAPI.RightsFilters
{
    /// <summary>
    /// Фильтрация пользователей по правам доступа.
    /// </summary>
    public interface IRightsFilter
    {
        /// <summary>
        /// Проверить права текущего пользователя.
        /// </summary>
        /// <param name="filterContext">Контекст</param>
        /// <param name="accesses">Требуемые доступы</param>
        Task<bool> CheckRightsAsync(ActionExecutingContext filterContext, int[] accesses);
    }
}