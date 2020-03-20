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

        private void Setup (int sameIdCount, int sceneCount = 1) {
            _generator = new SceneGenerator(sameIdCount, sceneCount);
        }

        [TearDown]
        public void Teardown () {
            _generator.Teardown();
        }

        private static void ClickButton (VisualElement root, string className) {
            var elView = root.Query<Button>(null, className).First();
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

        public class WhenClickingSearch : UniqueIdRepairWindowTest {
            [Test]
            public void It_should_display_a_searched_scene_path () {
                Setup(2);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;
                var title = GetText(root, "o-scene-search__title");

                Assert.AreEqual(_generator.ScenePaths[0], title);
            }

            [Test]
            public void It_should_print_the_number_searched_scenes () {
                Setup(1);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;
                var message = GetText(root, "p-window__message");

                Assert.IsTrue(message.Contains("1"));
            }

            [Test]
            public void It_should_print_a_message_when_no_errors_are_found () {
                Setup(1);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;
                var elText = root.Query<TextElement>(null, "test-p-window__no-errors").First();

                Assert.IsTrue(elText.ClassListContains("-show"));
            }

            [Test]
            public void It_should_not_print_no_found_error_message_by_default () {
                Setup(1);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Close();
                window = UniqueIdRepairWindow.ShowWindow();

                var root = window.rootVisualElement;
                var elText = root.Query<TextElement>(null, "test-p-window__no-errors").First();

                Assert.IsFalse(elText.ClassListContains("-show"));
            }

            [Test]
            public void It_should_clear_results_when_clicking_again () {
                Setup(2);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);
                window.Search(_generator.Path);

                var root = window.rootVisualElement;
                var el = root.Query<VisualElement>(null, "o-scene-search__results").First();

                Assert.AreEqual(2, el.Children().Count());
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

                var name = GetText(root, "m-unique-id-error__name");

                Assert.AreEqual(name, Selection.activeObject.name);
            }
        }

        public class ClickingFix : UniqueIdRepairWindowTest {
            [Test]
            public void It_should_fix_a_duplicate_id () {
                Setup(2);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;

                var corruptId = GetText(root, "m-unique-id-error__name");
                ClickButton(root, "m-unique-id-error__show");
                ClickButton(root, "m-unique-id-error__fix");

                var uniqueId = (Selection.activeObject as GameObject).GetComponent<UniqueId>();

                Assert.AreNotEqual(corruptId, uniqueId.Id);
            }

            [Test]
            public void It_should_mark_the_line_as_fixed () {
                Setup(2);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;

                ClickButton(root, "m-unique-id-error__fix");
                var elError = root.Query<VisualElement>(null, "m-unique-id-error").First();

                Assert.IsTrue(elError.ClassListContains("-fixed"));
            }

            [Test]
            public void It_should_mark_the_sibling_line_as_fixed () {
                Setup(2);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;

                ClickButton(root, "m-unique-id-error__fix");
                var elError = root.Query<VisualElement>(null, "m-unique-id-error").Last();

                Assert.IsTrue(elError.ClassListContains("-fixed"));
            }

            [Test]
            public void It_should_not_mark_a_duplicate_ID_as_fixed_if_there_is_two_or_more_unfixed_lines () {
                Setup(3);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;

                ClickButton(root, "m-unique-id-error__fix");
                var elError = root.Query<VisualElement>(null, "m-unique-id-error").Last();

                Assert.IsFalse(elError.ClassListContains("-fixed"));
            }
        }

        public class ClickingFixOnNull {
            public void It_should_fix_a_null_id () {

            }

            public void It_should_not_set_duplicate_null_ID_as_fixed_when_clicking_an_id () {

            }
        }
    }
}
