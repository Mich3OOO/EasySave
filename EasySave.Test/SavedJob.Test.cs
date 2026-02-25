using EasySave.Models;

namespace EasySave.Test
{
    [TestClass]
    public class TestSavedJob
    {
        // Paths created with Path.Combine to avoid horrible errors
        private static readonly string Src = Path.Combine(Path.GetTempPath(), "src");
        private static readonly string Dst = Path.Combine(Path.GetTempPath(), "dst");

        [TestMethod]
        public void TestNewJob()
        {
            var job = new SavedJob(1, "SaveJob", Src, Dst);

            Assert.AreEqual(1, job.Id);
            Assert.AreEqual("SaveJob", job.Name);
            Assert.AreEqual(Src, job.Source);
            Assert.AreEqual(Dst, job.Destination);
        }

        [TestMethod]
        public void TestCopyConstructor()
        {
            var original = new SavedJob(2, "Original", Src, Dst);
            var copy = new SavedJob(original);

            Assert.AreEqual("Original", original.Name);
            Assert.AreEqual("Original", copy.Name);

            copy.Name = "Modified";

            Assert.AreEqual("Modified", copy.Name);
        }

        [TestMethod]
        public void TestToString()
        {
            var job = new SavedJob(4, "WeeklyBackup", Src, Dst);
            Assert.IsTrue(job.ToString().Contains("WeeklyBackup"));
        }
    }
}
