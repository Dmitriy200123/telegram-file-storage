using FileStorageAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FileStorageAPI.Data
{
    /// <summary>
    /// Контекст отвечающий за данные об аутентифицированных пользователях
    /// </summary>
    public sealed class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Конструктор для создания необходим таблиц
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
