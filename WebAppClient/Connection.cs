using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace WebAppClient
{
    /// <summary>
    /// Подключение к серверу
    /// </summary>
    internal class Connection
    {
        /// <summary>
        /// Куки данные
        /// </summary>
        private CookieContainer cookies = new();
        private HttpClientHandler handler = new();
        private HttpClient client;
        private Uri ServerURL { get; }
        public Connection(Uri server_url)
        {
            client = new HttpClient(handler);
            handler.CookieContainer = cookies;
            client.BaseAddress = ServerURL = server_url;
        }

        /// <summary>
        /// Post request
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="content"></param>
        /// <returns>Ответ от сервера</returns>
        public AnswerServer<HttpResponseMessage> Post(string uri, HttpContent? content = null)
        {
            try
            {
                return PostAsync(uri, content).Result;
            }
            catch (Exception ex)
            {
                return AnswerServer<HttpResponseMessage>.Error(ex);
            }
        }
        /// <summary>
        /// async POST request
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="content"></param>
        /// <returns>Ответ от сервера</returns>
        public async Task<AnswerServer<HttpResponseMessage>> PostAsync(string uri, HttpContent? content = null)
        {
            try
            {
                var r = await client.PostAsync(uri, content);
                return AnswerServer<HttpResponseMessage>.Ok(r);
            }
            catch (Exception ex)
            {
                return AnswerServer<HttpResponseMessage>.Error(ex);
            }
        }
        public AnswerServer<string> ReadAnswerString(HttpContent httpContent)
        {
            try
            {
                return ReadAnswerStringAsync(httpContent).Result;
            }
            catch (Exception ex)
            {
                return AnswerServer<string>.Error(ex);
            }
        }
        public async Task<AnswerServer<string>> ReadAnswerStringAsync(HttpContent httpContent)
        {
            try
            {
                string res_content = await httpContent.ReadAsStringAsync();
                return AnswerServer<string>.Ok(res_content);
            }
            catch (Exception ex)
            {
                return AnswerServer<string>.Error(ex);
            }
        }
        public List<Cookie> GetAllCookies()
        {
            var response_Cookies = cookies.GetAllCookies();
            return new List<Cookie>(response_Cookies);
        }
    }
}
