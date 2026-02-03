using WebAppTest;
using WebAppClient;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Сервер Api - для работы функций WebAppTest
        /// </summary>
        private API_server Api;
        /// <summary>
        /// URL сервера WebAppTest
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
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestWriteGenerateArray_WithAnInputError()
        {
            const string userarray = "5 4  3 2 1 t";
            int[] benchmark_array = [5, 4, 0, 3, 2, 1, 0];
            var result = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestAddValueStart()
        {
            const string userarray = "5 4 3 2 1";
            const int value = 4;
            int[] benchmark_array = [4, 5, 4, 3, 2, 1];
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var resultAdd = Api.AddValueStart(value);
            Assert.IsTrue(resultAdd.Successful, resultAdd.ErrorData?.Message);
            var result = Api.GiveArray();

            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestAddValueFinish()
        {
            const string userarray = "5 4 3 2 1";
            const int value = 4;
            int[] benchmark_array = [5, 4, 3, 2, 1, 4];
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var resultAdd = Api.AddValueFinish(value);
            Assert.IsTrue(resultAdd.Successful, resultAdd.ErrorData?.Message);
            var result = Api.GiveArray();

            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestAddValueIndex()
        {
            const string userarray = "5 4 3 2 1";
            const int value = 4;
            const int index = 2;
            int[] benchmark_array = [5, 4, 3, 4, 2, 1];
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var resultAdd = Api.AddValueIndex(value, index);
            Assert.IsTrue(resultAdd.Successful, resultAdd.ErrorData?.Message);
            var result = Api.GiveArray();

            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestAddValueIndex_LessThanZero()
        {
            const string userarray = "5 4 3 2 1";
            const int value = 4;
            const int index = -2;
            int[] benchmark_array = [4, 5, 4, 3, 2, 1];
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var resultAdd = Api.AddValueIndex(value, index);
            Assert.IsTrue(resultAdd.Successful, resultAdd.ErrorData?.Message);
            var result = Api.GiveArray();

            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestAddValueIndex_MoreArray()
        {
            const string userarray = "5 4 3 2 1";
            const int value = 4;
            const int index = 222;
            int[] benchmark_array = [5, 4, 3, 2, 1, 4];
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var resultAdd = Api.AddValueIndex(value, index);
            Assert.IsTrue(resultAdd.Successful, resultAdd.ErrorData?.Message);
            var result = Api.GiveArray();

            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestGiveCombSort()
        {
            const string userarray = "5 4 3 2 1";
            int[] benchmark_array = [1, 2, 3, 4, 5];
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var result = Api.GiveCombSort();

            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestGiveCombSortIndex()
        {
            const string userarray = "5 4 3 2 1";
            const int start_index = 1;
            const int finish_index = 3;
            int[] benchmark_array = [2, 3, 4];
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var result = Api.GiveCombSortIndex(start_index, finish_index);

            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestWriteCombSort()
        {
            const string userarray = "5 4 3 2 1";
            int[] benchmark_array = [1, 2, 3, 4, 5];
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var resSort = Api.WriteCombSort();
            Assert.IsTrue(resSort.Successful, resSort.ErrorData?.Message);
            var result = Api.GiveArray();

            Assert.IsTrue(result.Successful, result.ErrorData?.Message);
            Assert.IsNotNull(result.Answer, "Answer is null");
            int i=0;
            int[] answer_array = result.Answer.Trim('[',']').Split(',')
            .Where((string item)=>int.TryParse(item, out i)).Select((_)=>i).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(answer_array, benchmark_array));
        }

        [TestMethod]
        public void TestDelArray()
        {
            const string userarray = "5 4 3 2 1";
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var resultDel = Api.DelArray();
            Assert.IsTrue(resultDel.Successful, resultDel.ErrorData?.Message);
            var result = Api.GiveArray();

            Assert.IsNull(result.Answer, "Answer is not null");
        }

        [TestMethod]
        public void TestUpdateHistory()
        {
            const string userarray = "5 4 3 2 1";
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);

            var newresult = Api.GetHistory();
            Assert.IsNotNull(newresult.Answer, "History is null");
        }

        [TestMethod]
        public void TestDelHistory()
        {
            const string userarray = "5 4 3 2 1";
            var resultWrite = Api.WriteGenerateArray(userarray);
            Assert.IsTrue(resultWrite.Successful, resultWrite.ErrorData?.Message);
            var resultDel = Api.DelHistory();
            Assert.IsTrue(resultDel.Successful, resultDel.ErrorData?.Message);
        }
    }
}
