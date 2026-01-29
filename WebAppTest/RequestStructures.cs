namespace WebAppTest
{
    //Структуры для запросов к серверу
    namespace RequestStructures
    {
        /// <summary>
        /// Запрос для генераторации случайного масива
        /// </summary>
        public struct Random_Generate_Array
        {
            public int low { get; set; }
            public int up { get; set; }
            public int count { get; set; }
        }

        /// <summary>
        /// Запрос для создания пользовательского масива
        /// </summary>
        public struct Write_Generate_Array
        {
            public string start_array { get; set; }
        }
        /// <summary>
        /// Данные пользователя
        /// </summary>
        public struct DataLogin
        {
            public string username { get; set; }
            public string password { get; set; }
        }


        //struct Borders
        //{
        //    public int low { get; set; }
        //    public int up { get; set; }
        //}
    }
}
