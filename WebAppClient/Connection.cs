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
        /// GET request
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="token">token</param>
        /// <returns>Ответ от сервера</returns>
        public AnswerServer<HttpResponseMessage> Get(string uri, CancellationToken? token = null)
        {
            try
            {
                return GetAsync(uri, token).Result;
            }
            catch (Exception ex)
            {
                return AnswerServer<HttpResponseMessage>.Error(ex);
            }
        }
        
        /// <summary>
        /// async GET request
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="token">token</param>
        /// <returns>Ответ от сервера</returns>
        public async Task<AnswerServer<HttpResponseMessage>> GetAsync(string uri, CancellationToken? token = null)
        {
            try
            {
                var r = await client.GetAsync(uri, token ?? CancellationToken.None);
                return AnswerServer<HttpResponseMessage>.Ok(r);
            }
            catch (Exception ex)
            {
                return AnswerServer<HttpResponseMessage>.Error(ex);
            }
        }
        
        /// <summary>
        /// POST request
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
        
        /// <summary>
        /// PATCH request
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="content"></param>
        /// <returns>Ответ от сервера</returns>
        public AnswerServer<HttpResponseMessage> Patch(string uri, HttpContent? content = null)
        {
            try { return PatchAsync(uri, content).Result; }
            catch (Exception ex) { return AnswerServer<HttpResponseMessage>.Error(ex); }
        }

        /// <summary>
        /// async PATCH request
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="content"></param>
        /// <returns>Ответ от сервера</returns>
        public async Task<AnswerServer<HttpResponseMessage>> PatchAsync(string uri, HttpContent? content = null)
        {
            try
            {
                var r = await client.PatchAsync(uri, content);
                return AnswerServer<HttpResponseMessage>.Ok(r);
            }
            catch (Exception ex)
            {
                return AnswerServer<HttpResponseMessage>.Error(ex);
            }
        }

        /// <summary>
        /// DELETE request
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="token">token</param>
        public AnswerServer<HttpResponseMessage> Delete(string uri, CancellationToken? token = null)
        {
            try { return DeleteAsync(uri, token).Result; }
            catch (Exception ex) { return AnswerServer<HttpResponseMessage>.Error(ex); }
        }

        /// <summary>
        /// async DELETE request
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="token">token</param>
        /// <returns>Ответ от сервера</returns>
        public async Task<AnswerServer<HttpResponseMessage>> DeleteAsync(string uri, CancellationToken? token = null)
        {
            try
            {
                var r = await client.DeleteAsync(uri, token ?? CancellationToken.None);
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
