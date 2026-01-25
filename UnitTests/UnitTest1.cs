namespace UnitTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestGetValue()
    {
        RandomGenerator random = new RandomGenerator();

        const int UPPER_BORDER = 10;
        const int LOWER_BORDER = 0;

        int result = random.GetValue();

        Assert.IsTrue(result >= LOWER_BORDER);
        Assert.IsTrue(result < UPPER_BORDER);
    }

    [TestMethod]
    public void TestGetValueParams()
    {
        RandomGenerator random = new RandomGenerator();

        const int UPPER_BORDER = 20;
        const int LOWER_BORDER = 10;

        int result = random.GetValue(LOWER_BORDER, UPPER_BORDER);

        Assert.IsTrue(result >= LOWER_BORDER);
        Assert.IsTrue(result < UPPER_BORDER);
    }

    [TestMethod]
    public void TestUpdateBorder()
    {
        RandomGenerator random = new RandomGenerator();

        const int UPPER_BORDER = 200;
        const int LOWER_BORDER = 100;

        random.UpdateBorder(LOWER_BORDER, UPPER_BORDER);

        int result = random.GetValue(LOWER_BORDER, UPPER_BORDER);

        Assert.IsTrue(result >= LOWER_BORDER);
        Assert.IsTrue(result < UPPER_BORDER);
    }
}