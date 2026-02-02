using WebAppTest;
using WebAppClient;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// ��� � ��������� ������������ (����)
        /// </summary>
        private API_server Api;
        /// <summary>
        /// ���� ��� ����������� � �������
        /// </summary>
        const string DEFAULT_SERVER_URL = "http://localhost:5114";
        public UnitTest1()
        {
            Api = new API_server(new Uri(DEFAULT_SERVER_URL));
            var answer = Api.LoginOnServer("1","1", showCookies: false);
            if(!answer.Successful) Assert.Fail("ERROR LOGIN!");
        }

        [TestMethod]
        public void TestRandomGenerateArray()
        {
            const int LOWER_BORDER = 0;
            const int UPPER_BORDER = 10;
            const int COUNT = 5;

            var result = Api.RandomGenerateArray(LOWER_BORDER, UPPER_BORDER, COUNT);
            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            (bool is_numb, bool lower, bool upper)[] conditions = result.Answer.Trim('[',']').Split(',')
            .Select((string item)=>(int.TryParse(item, out int i), LOWER_BORDER<=i, UPPER_BORDER>i)).ToArray(); // (string "15") => ((bool is int "true"), (bool 15>=lower "true"), (bool 15<upper "true"))=>(true,true,true)
            Assert.IsTrue(conditions.All(((bool is_numb, bool, bool) item)=> item.is_numb), "Ошибка преобразование массива");
            Assert.IsTrue(conditions.All(((bool, bool lower, bool upper) item)=>item.lower==item.upper==true), "Выход за границы");
        }

        [TestMethod]
        public void TestWriteGenerateArray()
        {
            const string userarray = "5 4 3 2 1";
            int[] benchmark_array = [5, 4, 3, 2, 1];
            var result = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            foreach (var item in answer_array) Console.Write($"{item} ");
            foreach (var item in benchmark_array) Console.Write($"{item} ");
            Assert.(answer_array, benchmark_array);
        }

        // [TestMethod]
        // public void TestGetValue()
        // {
        //     RandomGenerator random = new RandomGenerator();

        //     const int UPPER_BORDER = 10;
        //     const int LOWER_BORDER = 0;

        //     int result = random.GetValue();

        //     Assert.IsTrue(result >= LOWER_BORDER);
        //     Assert.IsTrue(result < UPPER_BORDER);
        // }

        // [TestMethod]
        // public void TestGetValueParams()
        // {
        //     RandomGenerator random = new RandomGenerator();

        //     const int UPPER_BORDER = 20;
        //     const int LOWER_BORDER = 10;

        //     int result = random.GetValue(LOWER_BORDER, UPPER_BORDER);

        //     Assert.IsTrue(result >= LOWER_BORDER);
        //     Assert.IsTrue(result < UPPER_BORDER);
        // }

        // [TestMethod]
        // public void TestUpdateBorder()
        // {
        //     RandomGenerator random = new RandomGenerator();

        //     const int UPPER_BORDER = 200;
        //     const int LOWER_BORDER = 100;

        //     random.UpdateBorder(LOWER_BORDER, UPPER_BORDER);

        //     int result = random.GetValue(LOWER_BORDER, UPPER_BORDER);

        //     Assert.IsTrue(result >= LOWER_BORDER);
        //     Assert.IsTrue(result < UPPER_BORDER);
        // }
    }
}
