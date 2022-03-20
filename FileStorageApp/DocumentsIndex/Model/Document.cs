using System;

namespace DocumentsIndex.Model
{
    public class Document
    {
        /// <summary>
        /// Все байты прочитанные из файла
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Id файла (эквивалентен Id, который лежит в БД и хранилище файлов)
        /// </summary>

        public Guid Id { get; set; }

        /// <summary>
        /// Название файла
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Конструктор создающий экземпляр класса Documentt
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="content">Данные из файла</param>
        /// <param name="name">Название файла</param>
        public Document(Guid id, byte[] content, string name)
        {
            Id = id;
            Content = content;
            Name = name;
        }
    }
}