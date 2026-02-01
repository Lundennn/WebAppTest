namespace WebAppTest
{
    //Структуры для запросов к серверу
    namespace RequestStructures
    {
        /// <summary>
        /// Для генераторации случайного масива
        /// </summary>
        public struct Random_Generate_Array
        {
            public int low { get; set; }
            public int up { get; set; }
            public int count { get; set; }
        }

        /// <summary>
        /// Для создания пользовательского массива
        /// </summary>
        public struct Write_Generate_Array
        {
            public string start_array { get; set; }
        }

        /// <summary>
        /// Для получения отсортированного массива от индекса до индекса
        /// </summary>
        public struct Array_Index
        {
            public int start_index { get; set; }
            public int finish_index { get; set; }
        }

        /// <summary>
        /// Для добавления элемента в массив
        /// </summary>
        public struct Array_Add_Value
        {
            public int value { get; set; }
        }

        /// <summary>
        /// Для добавления элемента в массив после указанного индекса
        /// </summary>
        public struct Array_Add_Index
        {
            public int value { get; set; }
            public int index { get; set; }
        }

        /// <summary>
        /// Данные пользователя
        /// </summary>
        public struct DataLogin
        {
            public string username { get; set; }
            public string password { get; set; }
        }
        /// <summary>
        /// Данные пользователя
        /// </summary>
        public struct New_Pwd
        {
            public string new_password { get; set; }
        }

        //struct Borders
        //{
        //    public int low { get; set; }
        //    public int up { get; set; }
        //}
    }
}
