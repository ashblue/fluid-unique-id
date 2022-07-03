using System.Linq;
using System.Reflection;
using CleverCrow.Fluid.UniqueIds.UniqueIdRepairs;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace CleverCrow.Fluid.UniqueIds {
    public class UniqueIdRepairWindowTest {
        private SceneGenerator _generator;

        private void Setup (int sameIdCount, int sceneCount = 1, int nullIdCount = 0) {
            _generator = new SceneGenerator(sameIdCount, sceneCount, nullIdCount);
        }

        [TearDown]
        public void Teardown () {
            _generator.Teardown();
        }

        private static void ClickButton (VisualElement root, string className) {
            var elView = GetElement<Button>(root, className);
            var viewClick = elView.clickable;
            var viewInvoke = viewClick
                .GetType()
                .GetMethod(
                    "Invoke",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

            viewInvoke.Invoke(viewClick, new object[] {MouseDownEvent.GetPooled()});
        }

        private static string GetText (VisualElement root, string className) {
            return root
                .Query<TextElement>(null, className)
                .First()
                .text;

        }

        private static T GetElement<T> (VisualElement root, string className) where T : VisualElement {
            var el = root
                .Query<T>(null, className)
                .First();

            Debug.Assert(el != null, $"Element {className} not found");

            return el;
        }

        public class WhenClickingSearch {
            public class ByDefault : UniqueIdRepairWindowTest {
                private VisualElement _root;

                private void SetupMethod (int sameIdCount) {
                    Setup(sameIdCount);

                    var window = UniqueIdRepairWindow.ShowWindow();
                    window.Search(_generator.Path);

                    _root = window.rootVisualElement;
                }

                [Test]
                public void It_should_display_a_searched_scene_path () {
                    SetupMethod(2);

                    var title = GetText(_root, "o-scene-search__title");

                    Assert.AreEqual(_generator.ScenePaths[0], title);
                }

                [Test]
                public void It_should_print_the_number_of_searched_scenes () {
                    SetupMethod(1);

                    var message = GetText(_root, "p-window__message");

                    Assert.IsTrue(message.Contains("1"));
                }

                [Test]
                public void It_should_print_a_message_when_no_errors_are_found () {
                    SetupMethod(1);

                    var elText = GetElement<TextElement>(_root, "test-p-window__no-errors");

                    Assert.IsTrue(elText.ClassListContains("-show"));
                }

                [Test]
                public void It_should_display_the_transform_path_to_the_error_object () {
                    SetupMethod(2);

                    ClickButton(_root, "m-unique-id-error__show");
                    var elName = GetElement<TextElement>(_root, "m-unique-id-error__name");

                    var expectedPath = ReportId.GetPath(Selection.activeObject as GameObject);

                    Assert.AreEqual(expectedPath, elName.text);
                }
            }

            public class WhenClearingResults : UniqueIdRepairWindowTest {
                [Test]
                public void It_should_not_print_no_found_error_message_by_default () {
                    Setup(1);

                    var window = UniqueIdRepairWindow.ShowWindow();
                    window.Close();
                    window = UniqueIdRepairWindow.ShowWindow();

                    var root = window.rootVisualElement;
                    var elText = GetElement<TextElement>(root, "test-p-window__no-errors");

                    Assert.IsFalse(elText.ClassListContains("-show"));
                }

                [Test]
                public void It_should_clear_results_when_clicking_again () {
                    Setup(2);

                    var window = UniqueIdRepairWindow.ShowWindow();
                    window.Search(_generator.Path);
                    window.Search(_generator.Path);

                    var root = window.rootVisualElement;
                    var el = GetElement<VisualElement>(root, "o-scene-search__results");

                    Assert.AreEqual(2, el.Children().Count());
                }
            }
        }

        public class ClickingShow : UniqueIdRepairWindowTest {
            [Test]
            public void It_should_set_the_current_selection_to_the_GameObject () {
                Setup(2);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;

                ClickButton(root, "m-unique-id-error__show");

                var id = GetText(root, "m-unique-id-error__text");

                Assert.AreEqual(id, (Selection.activeObject as GameObject).GetComponent<UniqueId>().Id);
            }
        }

        public class ClickingFix : UniqueIdRepairWindowTest {
            private VisualElement _root;
            private UniqueIdRepairWindow _window;

            private void MethodSetup (int sameIdCount) {
                Setup(sameIdCount);

                _window = UniqueIdRepairWindow.ShowWindow();
                _window.Search(_generator.Path);

                _root = _window.rootVisualElement;
            }

            [Test]
            public void It_should_fix_a_duplicate_id () {
                MethodSetup(2);

                var corruptId = GetText(_root, "m-unique-id-error__text");
                ClickButton(_root, "m-unique-id-error__show");
                ClickButton(_root, "m-unique-id-error__fix");

                var uniqueId = (Selection.activeObject as GameObject).GetComponent<UniqueId>();

                Assert.AreNotEqual(corruptId, uniqueId.Id);
            }

            [Test]
            public void It_should_update_the_display_id () {
                MethodSetup(2);

                var corruptId = GetText(_root, "m-unique-id-error__text");
                ClickButton(_root, "m-unique-id-error__fix");
                var newId = GetText(_root, "m-unique-id-error__text");

                Assert.AreNotEqual(corruptId, newId);
            }

            [Test]
            public void It_should_mark_the_line_as_fixed () {
                MethodSetup(2);

                ClickButton(_root, "m-unique-id-error__fix");
                var elError = GetElement<VisualElement>(_root, "m-unique-id-error");

                Assert.IsTrue(elError.ClassListContains("-fixed"));
            }

            [Test]
            public void It_should_mark_the_sibling_line_as_fixed () {
                MethodSetup(2);

                ClickButton(_root, "m-unique-id-error__fix");
                var elError = _root.Query<VisualElement>(null, "m-unique-id-error").Last();

                Assert.IsTrue(elError.ClassListContains("-fixed"));
            }

            [Test]
            public void It_should_not_mark_a_duplicate_ID_as_fixed_if_there_is_two_or_more_unfixed_lines () {
                MethodSetup(3);

                ClickButton(_root, "m-unique-id-error__fix");
                var elError = _root.Query<VisualElement>(null, "m-unique-id-error").Last();

                Assert.IsFalse(elError.ClassListContains("-fixed"));
            }
        }

        public class ClickingFixOnNullId : UniqueIdRepairWindowTest {
            private VisualElement _root;
            private UniqueIdRepairWindow _window;

            private void MethodSetup (int nullIdCount) {
                Setup(0, 1, nullIdCount);

                _window = UniqueIdRepairWindow.ShowWindow();
                _window.Search(_generator.Path);

                _root = _window.rootVisualElement;
            }

            [Test]
            public void It_should_print_null_as_the_id () {
                MethodSetup(1);

                var text = GetElement<TextElement>(_root, "m-unique-id-error__text").text;

                Assert.IsTrue(text.Contains("null"));
            }

            [Test]
            public void It_should_show_when_clicked () {
                MethodSetup(1);

                ClickButton(_root, "m-unique-id-error__show");
                var uniqueId = (Selection.activeObject as GameObject).GetComponent<UniqueId>();

                Assert.IsNotNull(uniqueId.Id);
            }

            [Test]
            public void It_should_fix_a_null_id () {
                MethodSetup(1);

                ClickButton(_root, "m-unique-id-error__show");
                ClickButton(_root, "m-unique-id-error__fix");
                var uniqueId = (Selection.activeObject as GameObject).GetComponent<UniqueId>();

                Assert.IsFalse(string.IsNullOrEmpty(uniqueId.Id));
            }

            [Test]
            public void It_should_not_set_sibling_id_to_fixed_when_clicking_an_id () {
                MethodSetup(2);

                ClickButton(_root, "m-unique-id-error__show");
                ClickButton(_root, "m-unique-id-error__fix");
                var elError = _root.Query<VisualElement>(null, "m-unique-id-error").Last();

                Assert.IsFalse(elError.ClassListContains("-fixed"));
            }
        }
    }
}
