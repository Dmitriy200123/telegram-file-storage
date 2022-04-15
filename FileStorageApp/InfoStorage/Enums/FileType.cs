using System.ComponentModel;

namespace FileStorageApp.Data.InfoStorage.Enums
{
    public enum FileType
    {
        [Description("Документ")]
        Document = 0,

        [Description("Аудио")]
        Audio = 1,

        [Description("Видео")]
        Video = 2,

        [Description("Изображение")]
        Image = 3,

        [Description("Ссылка")]
        Link = 4,

        [Description("Сообщение с пометкой")]
        Text = 5,
            
        [Description("Текстовый документ")]
        TextDocument = 6
    }
}