using NUnit.Framework;

namespace CleverCrow.Fluid.UniqueIds.Examples {
    public class ExampleVerifyIdTest {
        [Test]
        public void It_should_detect_invalid_IDs_in_the_project () {
            var reporter = new UniqueIdReporter("Assets/Examples");
            var report = reporter.GetReport();

            // Use this instead to enforce unique project IDs on your project, set to not error here for sanity reasons
            // Assert.AreEqual(0, report.ErrorCount);
            Assert.AreNotEqual(0, report.ErrorCount);
        }
    }
}
