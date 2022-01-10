using System.Linq;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorageAPI.RightsFilters
{
    /// <summary>
    /// Атрибут для контроля доступа пользователей к API.
    /// </summary>
    public class RightsFilterAttribute : ActionFilterAttribute
    {
        private readonly int[] _accesses;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="RightsFilterAttribute"/>.
        /// </summary>
        /// <param name="accesses">Требуемые доступы</param>
        public RightsFilterAttribute(params Accesses[] accesses)
        {
            _accesses = accesses.Cast<int>().ToArray();
        }

        /// <summary>
        /// Фильтрация.
        /// </summary>
        /// <param name="filterContext">Контекст</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var rightsFilter = filterContext.HttpContext.RequestServices.GetRequiredService<IRightsFilter>();

            if (!rightsFilter.CheckRights(filterContext, _accesses))
                filterContext.Result = new ForbidResult();
        }
    }
}