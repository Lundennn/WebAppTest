using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAppClient
{
    public class API_server
    {
        private Connection connection;
        public API_server(Uri uri)
        {
            connection = new Connection(uri);
        }

        public AnswerServer<bool> LoginOnServer(string username, string password, bool showCookies = true)
        {
            //Запрос
            string request = "/login";
            //Данные к запросу в формате json
            var json = new { username = username, password = password };
            //Объект который понимает Post
            var content = GetContentFromJSON(json);
            //Синхронное выполняется Post запроса к серверу
            var answer_server = connection.Post(request, content);

            var checkErr = CheckError<bool>(answer_server);
            if (checkErr != null) return checkErr;

            //Выводим куки
            if(showCookies)
            {
                //Выводим на экран все куки
                Console.WriteLine("All cookies:");
                foreach (var item in connection.GetAllCookies())
                    //Форматируем куки, чтобы можно было увидеть их в консоле
                    Console.WriteLine(CookieToSting(item));
            }
            //Отправляем ответ
            return AnswerServer<bool>.Ok(true);
        }
        
        internal AnswerServer<bool> SingUpOnServer(string username, string password)
        {
            string request = "/signup";
            var json = new { username = username, password = password };
            var content = GetContentFromJSON(json);
            var answer_server = connection.Post(request, content);
            var checkErr = CheckError<bool>(answer_server);
            return checkErr??AnswerServer<bool>.Ok(true);
        }

        public AnswerServer<string> RandomGenerateArray(int low, int up, int count)
        {
            string request = "/random_generate_array";
            var json = new { low = low, up = up, count = count };
            var content = GetContentFromJSON(json);
            var answer_server = connection.Post(request, content);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> WriteGenerateArray(string start_array)
        {
            string request = "/write_generate_array";
            var json = new { start_array = start_array };
            var content = GetContentFromJSON(json);
            var answer_server = connection.Post(request, content);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> AddValueStart(int value)
        {
            string request = "/add_value_start";
            var json = new { value = value };
            var content = GetContentFromJSON(json);
            var answer_server = connection.Patch(request, content);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> AddValueFinish(int value)
        {
            string request = "/add_value_finish";
            var json = new { value = value };
            var content = GetContentFromJSON(json);
            var answer_server = connection.Patch(request, content);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> AddValueIndex(int value, int index)
        {
            string request = "/add_value_index";
            var json = new { value = value, index = index };
            var content = GetContentFromJSON(json);
            var answer_server = connection.Patch(request, content);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> GiveArray()
        {
            string request = "/give_array";
            var answer_server = connection.Get(request);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> GiveCombSort()
        {
            string request = "/give_combsort";
            var answer_server = connection.Get(request);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> GetHistory()
        {
            string request = "/get_history";
            var answer_server = connection.Get(request);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> DelHistory()
        {
            string request = "/del_history";
            var answer_server = connection.Delete(request);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> GiveCombSortIndex(int start_index, int finish_index)
        {
            string request = $"/give_combsort_index?start_index={start_index}&finish_index={finish_index}";
            var answer_server = connection.Get(request); //Ошибка при Get
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> WriteCombSort()
        {
            string request = "/write_combsort";
            var answer_server = connection.Post(request);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> DelArray()
        {
            string request = "/del_array";
            var answer_server = connection.Delete(request);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        public AnswerServer<string> NewPassword(string new_password)
        {
            string request = "/new_pwd";
            var json = new { new_password = new_password };
            var content = GetContentFromJSON(json);
            var answer_server = connection.Post(request, content);
            var checkErr = CheckError<string>(answer_server);
            return checkErr ?? connection.ReadAnswerString(answer_server.Answer!.Content);
        }

        private AnswerServer<T>? CheckError<T>(AnswerServer<HttpResponseMessage> answer)
        {
            if (!answer.Successful)
                return AnswerServer<T>.Error(answer);

            if (!answer.Answer!.IsSuccessStatusCode)
            {
                var msg = connection.ReadAnswerString(answer.Answer!.Content).Answer;
                msg = string.IsNullOrEmpty(msg) ? "not msg" : msg;
                var code = (int)answer.Answer.StatusCode;
                return AnswerServer<T>.Error($"Is not success status code {code}, message: {msg}");
            }
            return null;
        }

        //Форматирование для того чтобы её можно было нормально посмотреть с консоли
        private string CookieToSting(Cookie cookie)
            => $"{cookie.Name} (Path:\"{cookie.Path}\") value: {cookie.Value}";

        //Преобразование объектов в JSON формат и запихиваем в формат "StringContent", чтобы он был как контент
        private StringContent GetContentFromJSON<T>(T json_data)
        {
            string jsonBody = JsonSerializer.Serialize(json_data);
            return new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
    }
}
