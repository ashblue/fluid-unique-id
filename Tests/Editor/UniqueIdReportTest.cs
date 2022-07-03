using NUnit.Framework;
using UnityEngine;

namespace CleverCrow.Fluid.UniqueIds {
    public class UniqueIdReportTest {
        public class GetReportMethod {
            private UniqueIdReporter _report;
            private string _id;
            private string _scenePath;
            private SceneGenerator _generator;

            private void Setup (int sameIdCount, int sceneCount = 1) {
                _generator = new SceneGenerator(sameIdCount, sceneCount);
                _scenePath = _generator.ScenePaths[0];
                _id = _generator.Id;

                _report = new UniqueIdReporter(_generator.Path);
            }

            [TearDown]
            public void Teardown () {
                _generator.Teardown();
            }

            public class SingleDuplicate : GetReportMethod {
                [SetUp]
                public void SetupMethod () {
                    Setup(1);
                }

                [Test]
                public void It_should_not_error_on_a_single_id () {
                    var result = _report.GetReport();

                    Assert.AreEqual(result.Scenes[0].Errors.Count, 0);
                }

                [Test]
                public void It_should_return_no_errors_when_there_are_none () {
                    var result = _report.GetReport();

                    Assert.AreEqual(result.ErrorCount, 0);
                }

                [Test]
                public void It_should_report_a_null_id_as_an_error () {
                    new GameObject("NullId").AddComponent<UniqueId>();
                    var result = _report.GetReport();

                    Assert.AreEqual(null, result.Scenes[0].Errors[0].Id);
                }
            }

            public class MultipleDuplicates : GetReportMethod {
                [SetUp]
                public void SetupMethod () {
                    Setup(2);
                }

                [Test]
                public void It_should_find_a_duplicate_id_in_the_same_scene () {
                    var result = _report.GetReport();

                    Assert.AreEqual(result.Scenes[0].Errors.Count, 2);
                }

                [Test]
                public void It_should_return_the_total_number_of_errors () {
                    var result = _report.GetReport();

                    Assert.AreEqual(result.ErrorCount, 2);
                }

                [Test]
                public void It_should_return_the_id_of_the_error () {
                    var result = _report.GetReport();

                    Assert.AreEqual(_id, result.Scenes[0].Errors[0].Id);
                }

                [Test]
                public void It_should_return_the_name_of_the_error_object () {
                    var result = _report.GetReport();

                    Assert.AreEqual("UniqueId(Clone)", result.Scenes[0].Errors[0].Name);
                }

                [Test]
                public void It_should_return_the_path_of_the_error_object () {
                    var result = _report.GetReport();

                    Assert.AreEqual("Container/UniqueId(Clone)", result.Scenes[0].Errors[0].Path);
                }
            }

            public class SingleDuplicateAcrossMultipleScenes : GetReportMethod {
                [SetUp]
                public void SetupMethod () {
                    Setup(1, 2);
                }

                [Test]
                public void It_should_find_an_error_in_the_first_scene () {
                    var result = _report.GetReport();

                    Assert.AreEqual(_id, result.Scenes[0].Errors[0].Id);
                }

                [Test]
                public void It_should_find_an_error_in_the_second_scene () {
                    var result = _report.GetReport();

                    Assert.AreEqual(_id, result.Scenes[1].Errors[0].Id);
                }

                [Test]
                public void It_should_detect_number_of_errors_across_scenes () {
                    var result = _report.GetReport();

                    Assert.AreEqual(2, result.ErrorCount);
                }
            }

            public class SceneDetails : GetReportMethod {
                [Test]
                public void It_should_report_the_scene_path () {
                    Setup(1);

                    var result = _report.GetReport();

                    Assert.AreEqual(_scenePath, result.Scenes[0].Path);
                }
            }
        }
    }
}
