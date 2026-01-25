using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.Arm;
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
        Console.WriteLine("Authorization was successful");
        IEnumerable<Cookie> response_Cookies = cookies.GetAllCookies();
        foreach (Cookie cookie in response_Cookies)
        {
            Console.WriteLine(cookie.Name + ": " + cookie.Value);
        }
        return true;
    }
    else
    {
        Console.WriteLine("Authorization failed");
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
        return "Data not available";
    }
}

string GetRandomWoForm(string? low, string? up)
{
    string request = "/random_params_form";

    var form = new MultipartFormDataContent();
    form.Add(new StringContent(low), "low");
    form.Add(new StringContent(up), "up");

    var response = client.PostAsync(request, form).Result;
    if (response.IsSuccessStatusCode) {
        return response.Content.ReadAsStringAsync().Result;
    }
    else
    {
        return "Data not available";
    }
}

string GetRandomWoHeader(string? low, string? up)
{
    string request_text = "/random_params_header";

    var request = new HttpRequestMessage(HttpMethod.Post, request_text);
    request.Headers.Add("low", low);
    request.Headers.Add("up", up);

    var response = client.SendAsync(request).Result;
    if (response.IsSuccessStatusCode) {
        return response.Content.ReadAsStringAsync().Result;
    }
    else
    {
        return "Data not available";
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
        return "Data not available";
    }
}

const string DEFAULT_SERVER_URL = "http://localhost:5000";
Console.WriteLine("Enter the server URL (http://localhost:5000 - default value)");
string? server_url = Console.ReadLine();

if (server_url == null || server_url.Length == 0)
{
    server_url = DEFAULT_SERVER_URL;

}

try
{
    client.BaseAddress = new Uri(server_url);

    //No SignIn

    Console.WriteLine("Enter your login and password:");
    string? username = Console.ReadLine();
    string? password = Console.ReadLine();

    if (!LoginOnServer(username, password))
    {
        Console.WriteLine("There was an error in your login or password. Please try again...");
        return;
    }

    Console.WriteLine(GetRandomWoParams());

    string? low = Console.ReadLine();
    string? up = Console.ReadLine();
    Console.WriteLine(GetRandomWoJson(low, up));
}
catch (Exception exp)
{
    Console.WriteLine("Errors: " + exp);
}