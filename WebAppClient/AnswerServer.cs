using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppClient
{
    /// <summary>
    /// Ответ от сервера
    /// </summary>
    /// <typeparam name="T">Тип ответа</typeparam>
    public class AnswerServer<T> : IAnswerServer_Error
    {
        /// <summary>
        /// Создать текстовую ошибку
        /// </summary>
        /// <param name="error_msg">Сообщение об ошибке</param>
        /// <returns></returns>
        static public AnswerServer<T> Error(string error_msg)
            => new AnswerServer<T>(false, new CustomException(error_msg), default(T));
        /// <summary>
        /// Создать ошибку
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        static public AnswerServer<T> Error(Exception ex)
            => new AnswerServer<T>(false, ex, default(T));
        /// <summary>
        /// Продублировать ошибку
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        static public AnswerServer<T> Error(IAnswerServer_Error ex)
            => new AnswerServer<T>(false, ex.ErrorData ?? new CustomException("Not specified"), default(T));
        /// <summary>
        /// Создать ответ от сервера
        /// </summary>
        /// <typeparam name="W">Тип ответа от сервера</typeparam>
        /// <param name="answer"></param>
        /// <returns></returns>
        static public AnswerServer<T> Ok(T answer)
            => new AnswerServer<T>(true, null, answer);
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="s">Успешность</param>
        /// <param name="e">Ошибка</param>
        /// <param name="a">Ответ от сервера</param>
        private AnswerServer(bool s, Exception? e, T? a)
        {
            Successful = s;
            ErrorData = e;
            Answer = a;
        }
        public bool Successful { get; }
        public Exception? ErrorData { get; }
        /// <summary>
        /// Ответ от сервера
        /// </summary>
        public T? Answer { get; }
        //public static implicit operator AnswerServer<T>(IAnswerServer_Error old)
        //{
        //    T? t = old.Answer is T ? (T)old.Answer : default(T);
        //    return new AnswerServer<T>(old.Successful, old.Error, t);
        //}
    }
    // Наследуем от Exception или его потомков
    public class CustomException : Exception
    {
        public CustomException(string message)
            : base(message) { }
    }
    public interface IAnswerServer_Error
    {
        /// <summary>
        /// Успешность
        /// </summary>
        public bool Successful { get; }
        /// <summary>
        /// Ошибка
        /// </summary>
        public Exception? ErrorData { get; }
    }
}
