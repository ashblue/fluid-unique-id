using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public class PageWindow : ComponentBase {
        private readonly List<Button> _buttons = new List<Button>();
        private readonly VisualElement _elSearchResults;
        private readonly List<SceneSearch> _searchedScenes = new List<SceneSearch>();
        private readonly TextElement _elSearchText;

        public PageWindow (VisualElement container) : base(container) {
            _elSearchResults = container.Query<VisualElement>("search-results").First();
            _elSearchText = container.Query<TextElement>("search-text").First();

            var buttons = container.Query<VisualElement>("buttons").First();
            _buttons.Add(new Button(buttons, "Search", Search));
        }

        private void Search () {
            if (!ConfirmSceneChange()) {
                return;
            }

            var startScenePath = SceneManager.GetActiveScene().path;
            ClearSearchResults();

            var idCounts = new Dictionary<string, int>();
            foreach (var scene in GetScenes()) {
                var sceneSearch = new SceneSearch(_elSearchResults, scene);
                sceneSearch.Records.ForEach(r => {
                    if (!idCounts.ContainsKey(r.id)) idCounts[r.id] = 0;
                    idCounts[r.id] += 1;
                });
                _searchedScenes.Add(sceneSearch);
            }

            _searchedScenes.ForEach(s => s.PrintDuplicates(idCounts));
            EditorSceneManager.OpenScene(startScenePath, OpenSceneMode.Single);

            _elSearchText.text = $"Searched {_searchedScenes.Count} scenes";
        }

        private void ClearSearchResults () {
            _searchedScenes.Clear();
            foreach (var child in _elSearchResults.Children().ToList()) {
                _elSearchResults.Remove(child);
            }
        }

        private List<SceneAsset> GetScenes () {
            var targets = Selection.GetFiltered(typeof(SceneAsset), SelectionMode.DeepAssets);
            return targets.Cast<SceneAsset>().ToList();
        }

        private bool ConfirmSceneChange () {
            return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
    }
}
