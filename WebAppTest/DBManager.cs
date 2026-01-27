using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;



public class DBManager
{
    public DBManager(string path)
    {
        Console.WriteLine("Connection to database...");
        try
        {
            connection = new SqliteConnection("Data Source=" + path);
            connection.Open();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                Console.WriteLine("Failed!");
                ConnectionBD = false;
                return;
            }
        }
        catch (Exception exp)
        {
            Console.WriteLine(exp.Message);
                ConnectionBD = false;
            return;
        }
        ConnectionBD = true;
        Console.WriteLine("Done!");
    }
    private SqliteConnection? connection = null;
    public bool ConnectionBD {get; private set;}
    private string HashPassword(string password)
    {
        using (var algoritm = SHA256.Create())
        {
            var bytes_hash = algoritm.ComputeHash(Encoding.Unicode.GetBytes(password));
            return Encoding.Unicode.GetString(bytes_hash);
        }
    }

    public void Disconnect()
    {
        if (null == connection)
            return;

        if (connection.State != System.Data.ConnectionState.Open)
            return;

        connection.Close();

        Console.WriteLine("Disconnect from database");
    }

    public bool AddUser(string login, string password)
    {
        if (null == connection)
            return false;

        if (connection.State != System.Data.ConnectionState.Open)
            return false;

        string REQUEST = "INSERT INTO users (Login, Password) VALUES ('" + login + "', '" + HashPassword(password) + "')";
        var command = new SqliteCommand(REQUEST, connection);

        int result = 0;

        try
        {
            result = command.ExecuteNonQuery();
        }
        catch (Exception exp)
        {
            Console.WriteLine(exp.Message);
            return false;
        }

        if (1 == result)
            return true;
        else
            return false;
    }
    public bool AddArr(string login, int[] ints)
    {
        if (!CheckConnect) return false;
        string json = "null";
        if(ints != null && ints.Length != 0)
            json = System.Text.Json.JsonSerializer.Serialize(ints);
        string REQUEST = $"UPDATE users SET array_data = '{json}' WHERE Login = '{login}';";
        var command = new SqliteCommand(REQUEST, connection);

        try
        {
            var reader = command.ExecuteReader();

            if (reader.HasRows)
                return true;
            else
                return false;
        }
        catch (Exception exp)
        {
            Console.WriteLine(exp.Message);
            return false;
        }
    }
    private bool CheckConnect => connection != null && connection.State == System.Data.ConnectionState.Open;
    public bool CheckUser(string login, string password)
    {
        if (null == connection)
            return false;

        if (connection.State != System.Data.ConnectionState.Open)
            return false;

        string REQUEST = "SELECT Login,Password FROM users WHERE Login='" + login + "' AND Password='" + HashPassword(password) + "'";
        var command = new SqliteCommand(REQUEST, connection);

        try
        {
            var reader = command.ExecuteReader();

            if (reader.HasRows)
                return true;
            else
                return false;
        }
        catch (Exception exp)
        {
            Console.WriteLine(exp.Message);
            return false;
        }
    }
}