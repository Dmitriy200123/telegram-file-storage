using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentsIndex.Contracts
{
    public class Document
    {
        /// <summary>
        /// Все байты прочитанные из файла
        /// </summary>
        [Required]
        public byte[] Content { get; set; }

        /// <summary>
        /// Id файла (эквивалентен Id, который лежит в БД и хранилище файлов)
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Название файла
        /// </summary>
        [Required]
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
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}