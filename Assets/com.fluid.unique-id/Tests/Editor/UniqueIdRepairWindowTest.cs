using System.Linq;
using CleverCrow.Fluid.UniqueIds.UniqueIdRepairs;
using NUnit.Framework;
using UnityEngine.UIElements;

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

        public class WhenClickingSearch : UniqueIdRepairWindowTest {
            [Test]
            public void It_should_display_a_searched_scene_path () {
                Setup(2);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;
                var elText = root.Query<TextElement>(null, "o-scene-search__title").First();

                Assert.AreEqual(_generator.ScenePaths[0], elText.text);
            }

            [Test]
            public void It_should_print_the_number_searched_scenes () {
                Setup(1);

                var window = UniqueIdRepairWindow.ShowWindow();
                window.Search(_generator.Path);

                var root = window.rootVisualElement;
                var elText = root.Query<TextElement>(null, "p-window__message").First();

                Assert.IsTrue(elText.text.Contains("1"));
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

        public class ClickingShow {
            public void It_should_set_the_current_selection_to_the_GameObject () {

            }
        }

        public class ClickingFix {
            public void It_should_fix_a_duplicate_id () {

            }

            public void It_should_mark_the_line_as_fixed () {

            }

            public void It_should_mark_the_sibling_line_as_fixed () {

            }

            public void It_should_not_mark_a_duplicate_ID_as_fixed_if_there_is_two_or_more_unfixed_lines () {

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
