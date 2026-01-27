using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

CookieContainer cookies = new CookieContainer();
HttpClientHandler handler = new HttpClientHandler();
HttpClient client = new HttpClient(handler);
handler.CookieContainer = cookies;

bool LoginOnServer(string? username, string? password)
{
    if (username == null || username.Length == 0 ||
    password == null || password.Length == 0)
    {
        return false;
    }

    string request = "/login?login=" + username + "&password="+ password;
    var response = client.PostAsync(request, null).Result;
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Авторизация прошла успешно!");
        IEnumerable<Cookie> response_Cookies = cookies.GetAllCookies();
        foreach (Cookie cookie in response_Cookies)
        {
            Console.WriteLine(cookie.Name + ": " + cookie.Value);
        }
        return true;
    }
    else
    {
        Console.WriteLine("Не удалось авторизоваться");
        return false;
    }
}

string GetRandomWoParams()
{
    string request = "/random";
    var response = client.GetAsync(request).Result;
    if (response.IsSuccessStatusCode) {
        return response.Content.ReadAsStringAsync().Result;
    }
    else
    {
        return "Данные не доступны";
    }
}

string GetRandomWoJson(string _low, string _up)
{
    string request = "/random_params_json";

    var json_data = new
    {
        low = int.Parse(_low),
        up = int.Parse(_up)
    };
    string jsonBody = JsonSerializer.Serialize(json_data);
    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

    var response = client.PostAsync(request, content).Result;
    if (response.IsSuccessStatusCode) {
        return response.Content.ReadAsStringAsync().Result;
    }
    else
    {
        return "Данные не доступны";
    }
}

StartTest();
const string DEFAULT_SERVER_URL = "http://localhost:5000";
Console.WriteLine("Введите URL сервера (http://localhost:5000 - по умолчанию)");

string? server_url = Console.ReadLine();

if (server_url == null || server_url.Length == 0)
{
    server_url = DEFAULT_SERVER_URL;

}

try
{
    client.BaseAddress = new Uri(server_url);

    Console.WriteLine("======================================");
    Console.WriteLine("              МЕНЮ ВХОДА              ");
    Console.WriteLine("======================================");
    Console.WriteLine("   1. Авторизоваться                  ");
    Console.WriteLine("   2. Зарегистрироваться              ");
    Console.WriteLine("   0. Выход                           ");
    Console.WriteLine("======================================");

    string? menu_login_value = Console.ReadLine();
    Console.Clear();
    if (menu_login_value == "1")
    {
        Console.WriteLine("======================================");
        Console.WriteLine("            Авторизация...            ");
        Console.WriteLine("======================================");
        Console.Write("   Введите логин: ");
        string? username = Console.ReadLine();
        Console.Write("   Введите пароль: ");
        string? password = Console.ReadLine();
        Console.WriteLine("======================================");

        if (!LoginOnServer(username, password))
        {
            Console.WriteLine("Не верный логин/пароль, попробуйте снова...");
            return;
        }

        Console.WriteLine(GetRandomWoParams());

        string? low = Console.ReadLine();
        string? up = Console.ReadLine();
        Console.WriteLine(GetRandomWoJson(low, up));    
    }
    if (menu_login_value == "2") {}
    if (menu_login_value == "0")
    {
        return;
    }
}
catch (Exception exp)
{
    Console.WriteLine("Ошибка: " + exp);
}



void StartTest()
{
    const string def_server = "http://localhost:5000";
    client.BaseAddress = new Uri(def_server);
    const string username = "user222";
    const string password = "password222";
    
    if (!LoginOnServer(username, password))
    {
        Console.WriteLine("!!!Error connect!!!");
        return;
    }
    // var res = PostRandomGenerateArray(88, 99, 5) ? "successful" : "error";
    string text = "88 99 5 33 44";
    var res = PostWriteGenerateArray(text) ? "successful" : "error";
    Console.WriteLine(res);
}

bool PostRandomGenerateArray(int _low, int _up, int _count) //Радомная генерация массива
{
    string request = "/random_generate_array";

    var json_data = new
    {
        low = _low,
        up = _up,
        count = _count
    };
    string jsonBody = JsonSerializer.Serialize(json_data);
    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

    var response = client.PostAsync(request, content).Result;
    if (!response.IsSuccessStatusCode) return false;
    var r = response.Content.ReadAsStringAsync().Result;
    Console.WriteLine(r);
    return true;
}

bool PostWriteGenerateArray(string _start_array) //Ручная генерация массива
{
    string request = "/write_generate_array";

    var json_data = new
    {
        start_array = _start_array
    };
    string jsonBody = JsonSerializer.Serialize(json_data);
    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

    var response = client.PostAsync(request, content).Result;
    if (!response.IsSuccessStatusCode) return false;
    var r = response.Content.ReadAsStringAsync().Result;
    Console.WriteLine(r);
    return true;
}

// bool Post_RandNewArr(int _low, int _up, int _count)
// {
//     string request = "/test";

//     var json_data = new
//     {
//         low = _low,
//         up = _up,
//         count = _count
//     };
//     string jsonBody = JsonSerializer.Serialize(json_data);
//     var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//     var response = client.PostAsync(request, content).Result;
//     if (!response.IsSuccessStatusCode) return false;
//     var r = response.Content.ReadAsStringAsync().Result;
//     Console.WriteLine(r);
//     return true;
// }
