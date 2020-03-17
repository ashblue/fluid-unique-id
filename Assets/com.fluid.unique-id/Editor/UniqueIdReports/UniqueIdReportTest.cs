using System;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CleverCrow.Fluid.UniqueIds {
    public class UniqueIdReportTest {
        public class GetReportMethod {
            private UniqueIdReporter _report;
            private string _id;

            private void Setup (int sameIdCount) {
                var tmpFolder = Guid.NewGuid().ToString();
                AssetDatabase.CreateFolder("Assets", tmpFolder);

                var tmpScene = $"Assets/{tmpFolder}/{Guid.NewGuid()}.unity";
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                var go = new GameObject("UniqueId");
                var id = go.AddComponent<UniqueId>();
                id.PopulateIdIfEmpty();
                _id = id.Id;

                sameIdCount -= 1;
                while (sameIdCount > 0) {
                    Object.Instantiate(id);
                    sameIdCount -= 1;
                }

                EditorSceneManager.SaveScene(scene, tmpScene);

                _report = new UniqueIdReporter($"Assets/{tmpFolder}");
            }

            [TearDown]
            public void Teardown () {
                AssetDatabase.DeleteAsset(_report.Path);
            }

            [Test]
            public void It_should_find_a_duplicate_id_in_the_same_scene () {
                Setup(2);

                var result = _report.GetReport();

                Assert.AreEqual(result.Scenes[0].Errors.Count, 2);
            }

            [Test]
            public void It_should_not_error_on_a_single_id () {
                Setup(1);

                var result = _report.GetReport();

                Assert.AreEqual(result.Scenes[0].Errors.Count, 0);
            }

            [Test]
            public void It_should_return_the_total_number_of_errors () {
                Setup(2);

                var result = _report.GetReport();

                Assert.AreEqual(result.ErrorCount, 2);
            }

            [Test]
            public void It_should_return_no_errors_when_there_are_none () {
                Setup(1);

                var result = _report.GetReport();

                Assert.AreEqual(result.ErrorCount, 0);
            }

            [Test]
            public void It_should_return_the_id_of_the_error () {
                Setup(2);

                var result = _report.GetReport();

                Assert.AreEqual(_id, result.Scenes[0].Errors[0].Id);
            }

            [Test]
            public void It_should_return_the_name_of_the_error_object () {
                Setup(2);

                var result = _report.GetReport();

                Assert.AreEqual("UniqueId(Clone)", result.Scenes[0].Errors[0].Name);
            }
        }
    }
}
