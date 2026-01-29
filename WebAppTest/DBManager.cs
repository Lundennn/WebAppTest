using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;


namespace WebAppTest
{
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
            ConnectionBD = false;

            Console.WriteLine("Disconnect from database");
        }

        public bool NewPassword(string login, string pwd)
        {
            if (!ConnectionBD) return false;

            string REQUEST = $"UPDATE users SET Password = '{HashPassword(pwd)}' WHERE login = '{login}';";
            var command = new SqliteCommand(REQUEST, connection);   
            try
            {
                return command.ExecuteNonQuery() == 1;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return false;
            }
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

        public bool DelArr(string login)
        {
            if (!CheckConnect) return false;

            string REQUEST = $"UPDATE users SET array_data = 'null' WHERE Login = '{login}';";
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

        public int[]? GiveArray(string login)
        {
            string REQUEST = "SELECT array_data FROM Users WHERE Login='" + login + "'";
            var command = new SqliteCommand(REQUEST, connection);
            try
            {
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var json = reader.GetString(0);
                    return System.Text.Json.JsonSerializer.Deserialize<int[]>(json!)!;
                }
                else
                    return null;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return null;
            }
        }
        public bool CheckUser(string login)
        {
            string REQUEST = "SELECT 1 FROM users WHERE login = '" + login + "' LIMIT 1;";
            var command = new SqliteCommand(REQUEST, connection);
            try
            {
                var reader = command.ExecuteReader();
                return reader.HasRows;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return false;
            }
        }
        public class Record
        {
            public const string FormatDate = "dd.mm.yy -- ss:MM:hh";
            public Record(string apiMethod, bool isSuccessefule, string data)
            {
                Date = DateTime.Now;
                ApiMethod = apiMethod;
                IsSuccessefule = isSuccessefule;
                Data= data;
            }
            public DateTime Date {get; set; }
            public string ApiMethod {get; set; }
            public bool IsSuccessefule {get; set; }
            public string Data {get;set;}
            public override string ToString()
                =>$"[{Date.ToString(FormatDate)} {ApiMethod} res-{IsSuccessefule}: {Data}]";
        }
        public bool UpDateHistory(string login, Record record)
        {
            if (!CheckConnect) return false;
            string REQUEST = "SELECT history FROM Users WHERE Login='" + login + "' LIMIT 1;";
            var command = new SqliteCommand(REQUEST, connection);
            List<Record> all_records;
            try
            {
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var json = reader.GetString(0);
                    if(string.IsNullOrEmpty(json) || json == "null")
                        all_records = new(); 
                    else
                        all_records = System.Text.Json.JsonSerializer.Deserialize<List<Record>>(json!)!;
                }
                else
                    return false;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return false;
            }
            all_records.Add(record);
            string new_json = System.Text.Json.JsonSerializer.Serialize(all_records)!;
            REQUEST = $"UPDATE users SET history = '{new_json}' WHERE Login = '{login}'  LIMIT 1;";
            command = new SqliteCommand(REQUEST, connection);
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
        public string[]? GetHistory(string login)
        {
            if (!CheckConnect) return null;
            string REQUEST = "SELECT history FROM Users WHERE Login='" + login + "' LIMIT 1;";
            var command = new SqliteCommand(REQUEST, connection);
            try
            {
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var json = reader.GetString(0);
                    if(string.IsNullOrEmpty(json) || json == "null")
                    {
                        return null;//nuint
                    }
                    var history = System.Text.Json.JsonSerializer.Deserialize<List<Record>>(json!);
                    return history?.Select(item => item.ToString()).ToArray();
                }
                else
                    return null;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return null;
            }
        }
        public bool DelHistory(string login)
        {
            if (!CheckConnect) return false;

            string REQUEST = $"UPDATE users SET history = 'null' WHERE Login = '{login}' LIMIT 1;";
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
        // public bool UpdateArray(string login, int[] ints)
        // {
        //     if (!CheckConnect) return false;
        //     string json = "null";
        //     if(ints != null && ints.Length != 0)
        //         json = System.Text.Json.JsonSerializer.Serialize(ints);
        //     string REQUEST = $"UPDATE users SET array_data = '{json}' WHERE Login = '{login}';";
        //     var command = new SqliteCommand(REQUEST, connection);

        //     try
        //     {
        //         var reader = command.ExecuteReader();

        //         if (reader.HasRows)
        //             return true;
        //         else
        //             return false;
        //     }
        //     catch (Exception exp)
        //     {
        //         Console.WriteLine(exp.Message);
        //         return false;
        //     }
        // }
    }
}