using System;
using System.Collections.Generic;
using System.Net;

namespace API
{
    /// <summary>
    /// Класс для упрощения взаимодействия между контроллерами и сервисами.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestResult<T>
    {
        /// <summary>
        /// Значение, которое есть, если сервис смог успешно отработать.
        /// </summary>
        public readonly T? Value;

        /// <summary>
        /// Код ответа, который стоит вернуть контроллеру.
        /// </summary>
        public readonly HttpStatusCode ResponseCode;

        /// <summary>
        /// Сообщение которое должен вернуть контроллер
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Словарь дополнительных параметров
        /// </summary>
        public readonly Dictionary<ExtraOption, string>? ExtraOptions;

        /// <summary>
        /// Конструктор для случая, когда не получилось успешно выполнить действия.
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="message"></param>
        /// <param name="value"></param>
        /// <param name="extraOptions"></param>
        public RequestResult(
            HttpStatusCode responseCode,
            string message,
            T value = default!,
            Dictionary<ExtraOption, string> extraOptions = default
        )
        {
            ResponseCode = responseCode;
            Message = message;
            Value = value;
            ExtraOptions = extraOptions;
        }

        /// <summary>
        /// Конструктор, когда сервис успешно выполнил действия.
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public RequestResult(
            HttpStatusCode responseCode,
            T value,
            string message = "Success",
            Dictionary<ExtraOption, string> extraOptions = default
        )
        {
            ResponseCode = responseCode;
            Message = message;
            Value = value;
            ExtraOptions = extraOptions;
        }
    }

    /// <summary>
    /// Синтаксический сахар для динамического класса RequestResult.
    /// при необходимости можно дописать свои методы
    /// </summary>
    public static class RequestResult
    {
        /// <summary>
        /// Следует использовать, когда не получилось найти какие-то данные или запрос был неверным.
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="T">Тип, который возвращает сервис</typeparam>
        public static RequestResult<T> NotFound<T>(string message)
        {
            return new RequestResult<T>(HttpStatusCode.NotFound, message);
        }

        /// <summary>
        /// Следует использовать, когда не получилось найти какую-то сущность.
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="entityName">Название сущности</param>
        /// <typeparam name="T">Тип, который возвращает сервис</typeparam>
        public static RequestResult<T> NotFoundEntity<T>(string message, string entityName)
        {
            var extraOptions = new Dictionary<ExtraOption, string> { [ExtraOption.EntityName] = entityName };

            return new RequestResult<T>(HttpStatusCode.NotFound, message, extraOptions: extraOptions);
        }
        

        /// <summary>
        /// Следует использовать, когда код успешно отработал.
        /// </summary>
        /// <param name="value">Значение, которое возвращает</param>
        /// <typeparam name="T">Тип, который возвращает </typeparam>
        public static RequestResult<T> Ok<T>(T value)
        {
            return new RequestResult<T>(HttpStatusCode.OK, value);
        }

        /// <summary>
        /// Следует использовать, когда что-то было удалено.
        /// </summary>
        /// <typeparam name="T">Тип, который был удален</typeparam>
        public static RequestResult<T> NoContent<T>()
        {
            return new RequestResult<T>(HttpStatusCode.NoContent, value: default!);
        }

        /// <summary>
        /// Следует использовать, когда что-то было создано.
        /// </summary>
        /// <param name="value">То, что было создано</param>
        /// <typeparam name="T">Тип, который был создан</typeparam>
        public static RequestResult<T> Created<T>(T value)
        {
            return new RequestResult<T>(HttpStatusCode.Created, value);
        }

        /// <summary>
        /// Следует использовать, когда на беке что-то пошло не так.
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        public static RequestResult<T> InternalServerError<T>(string message)
        {
            return new RequestResult<T>(HttpStatusCode.InternalServerError, message);
        }

        /// <summary>
        /// Следует использовать, когда запрос некорректный.
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        public static RequestResult<T> BadRequest<T>(string message)
        {
            return new RequestResult<T>(HttpStatusCode.BadRequest, message);
        }

        /// <summary>
        /// Следует использовать, когда пользователь не авторизован
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static RequestResult<T> Unauthorized<T>(string message)
        {
            return new RequestResult<T>(HttpStatusCode.Unauthorized, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static RequestResult<T> Forbidden<T>(string message)
        {
            return new RequestResult<T>(HttpStatusCode.Forbidden, message);
        }

        public static RequestResult<T2> EditReturnValueIfExist<T2, T>(this RequestResult<T> basicRequest, Func<T?, T2> func)
        {
            if (basicRequest.Value == null)
                return new RequestResult<T2>(basicRequest.ResponseCode, basicRequest.Message);
            
            var changedValue = func(basicRequest.Value);
            return new RequestResult<T2>(basicRequest.ResponseCode, changedValue);
        }
    }
}