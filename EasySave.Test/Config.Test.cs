using EasySave.Models;

namespace EasySave.Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class TestConfig
    {
        private readonly Config _config = Config.GetInstance();

        // Paths created with Path.Combine to avoid horrible errors
        private static readonly string TempSrc = Path.Combine(Path.GetTempPath(), "src");
        private static readonly string TempDst = Path.Combine(Path.GetTempPath(), "dst");

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
        public void TestAddJob()
        {
            var job = new SavedJob(101, "UnitTest_AddJob", TempSrc, TempDst);
            bool added = _config.AddJob(job);
            Assert.IsTrue(added);
            _config.DeleteJob(job);
        }

        [TestMethod]
        public void TestAddJob_Duplicate()
        {
            var job = new SavedJob(102, "UnitTest_Duplicate", TempSrc, TempDst);
            _config.AddJob(job);

            bool addedAgain = _config.AddJob(job);
            Assert.IsFalse(addedAgain);

            _config.DeleteJob(job);
        }

        [TestMethod]
        public void TestGetJob_Null()
        {
            var result = _config.GetJob("this_job_does_not_exist_feur");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeleteJob()
        {
            var job = new SavedJob(103, "UnitTest_Delete", TempSrc, TempDst);
            _config.AddJob(job);
            _config.DeleteJob(job);

            Assert.IsNull(_config.GetJob("UnitTest_Delete"));
        }
    }
}
