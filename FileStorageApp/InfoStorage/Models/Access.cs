using System.ComponentModel;

namespace FileStorageApp.Data.InfoStorage.Models
{
    /// <summary>
    /// Права пользователей на различные функции в сервисе.
    /// </summary>
    public enum Access
    {
        /// <summary>
        /// Дефолтные права для всех пользователей, нужно чтобы кто-то случайно не занял ноль и потом не стрельнул себе в ногу
        /// </summary>
        Default = 0,

        /// <summary>
        /// Возможность загружать файлы
        /// </summary>
        [Description("Загружать файлы")]
        Upload = 1,

        /// <summary>
        /// Возможность переименовывать файлы
        /// </summary>
        [Description("Переименовывать файлы")]
        Rename = 2,

        /// <summary>
        /// Возможность удалять файлы
        /// </summary>
        [Description("Удалять файлы")]
        Delete = 3,

        /// <summary>
        /// Возможность управления пользователями и их правами
        /// </summary>
        [Description("Управлять пользователями и их правами")]
        UserAccessesManagement = 4,
        
        /// <summary>
        /// Возможность просматривать любые файлы
        /// </summary>
        [Description("Просматривать файлы из любых чатов")]
        ViewAnyFiles = 5
    }
}