using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace RightServices
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
        public RightsFilterAttribute(params Access[] accesses)
        {
            _accesses = accesses.Cast<int>().ToArray();
        }

        /// <summary>
        /// Фильтрация.
        /// </summary>
        /// <param name="context">Контекст</param>
        /// <param name="next">Делегат</param>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rightsFilter = context.HttpContext.RequestServices.GetRequiredService<IRightsFilter>();
            var isCorrect = await rightsFilter.CheckRightsAsync(context, _accesses);

            if (!isCorrect)
                context.Result = new ForbidResult();

            await base.OnActionExecutionAsync(context, next);
        }
    }
}