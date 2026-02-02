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
            //Тут пишешь запрос
            string request = "/login";
            //Тут пишешь данные к запросу в формате json
            var json = new { username = username, password = password };
            //Тут оно превращается в объект которые понимает Post
            var content = GetContentFromJSON(json);
            //Тут синхронно выполняется Post запрос к серверу
            var answer_server = connection.Post(request, content);

            var checkErr = CheckError<bool>(answer_server);
            if (checkErr != null) return checkErr;

            //Выводим куки если нужно
            if(showCookies)
            {
                //Тут мы выводим на экран усе куки
                Console.WriteLine("All cookies:");
                foreach (var item in connection.GetAllCookies())
                    //Тут мы форматируем куку, чтобы можно было её увидеть в консоле
                    Console.WriteLine(CookieToSting(item));
            }
            //Усё, отправляем что мы молодцы и сделали усё правильно
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
            //Туть усё в целом всё тоже самое
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
            //Тут ловятся ошибки об проблемах в соединении с сервером
            if (!answer.Successful)
                //Тут эта ошибка просто отправляется как результат
                return AnswerServer<T>.Error(answer);
            //Тут ловится ошибка связанная с неверным кодом ответа, всё что не 200 (успех - successeful)
            if (!answer.Answer!.IsSuccessStatusCode)
            {
                //Туть мы считываем сообщение об ошибке от сервера
                var msg = connection.ReadAnswerString(answer.Answer!.Content).Answer;
                msg = string.IsNullOrEmpty(msg) ? "not msg" : msg;
                //Тута считываем какой код прислал сервер
                var code = (int)answer.Answer.StatusCode;
                //Тут мы должны лапками об этом написать, что код не 2XX и выдать сообщение от сервера
                return AnswerServer<T>.Error($"Is not success status code {code}, message: {msg}");
            }
            return null;
        }

        //Тут мы форматируем куку для того чтобы её можно было нормально посмотреть с консоли
        private string CookieToSting(Cookie cookie)
            => $"{cookie.Name} (Path:\"{cookie.Path}\") value: {cookie.Value}";

        //Тут мы превращаем объекты в JSON формат и запихиваем в формат "StringContent", чтобы он был как контент
        private StringContent GetContentFromJSON<T>(T json_data)
        {
            string jsonBody = JsonSerializer.Serialize(json_data);
            return new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
    }
}
