using System.Net;

namespace FileStorageAPI
{
    public class RequestResult<T>
    {
        public T? Value;
        public HttpStatusCode ResponseCode;
        public string Message;

        public RequestResult(HttpStatusCode responseCode, string message, T value = default)
        {
            ResponseCode = responseCode;
            Message = message;
            Value = value;
        }
        public RequestResult(HttpStatusCode responseCode,T value, string message = "Success")
        {
            ResponseCode = responseCode;
            Message = message;
            Value = value;
        }
    }

    public static class RequestResult
    {
        public static RequestResult<T> NotFound<T>(string message)
        {
            return new RequestResult<T>(HttpStatusCode.NotFound, message);
        }

        public static RequestResult<T> Ok<T>(T value)
        {
            return new RequestResult<T>(HttpStatusCode.OK, value);
        }
    }
}