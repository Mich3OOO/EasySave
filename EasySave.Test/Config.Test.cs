using EasySave.Models;
namespace EasySave.Test
{
    [TestClass]
    public sealed class TestConfig
    {
        private readonly Config _config = Config.GetInstance();

        [TestMethod]
        public void TestSetDefaultConfig()
        {
        }
    }
}
