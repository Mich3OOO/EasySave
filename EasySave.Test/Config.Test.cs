using EasySave.Models;

namespace EasySave.Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class TestConfig
    {
        private readonly Config _config = Config.GetInstance();

        [TestMethod]
        public void TestSetConfig()
        {
            _config.Language = Languages.En;
            _config.LogsFormat = LogsFormats.Json;

            Assert.AreEqual(Languages.En, _config.Language);
            Assert.AreEqual(LogsFormats.Json, _config.LogsFormat);
        }

        [TestMethod]
        public void TestGetInstance()
        {
            var instance = Config.GetInstance();
            Assert.AreSame(_config, instance);
        }

        [TestMethod]
        public void TestGetJob_Null()
        {
            var result = _config.GetJob("this_job_does_not_exist_feur");
            Assert.IsNull(result);
        }
    }
}
