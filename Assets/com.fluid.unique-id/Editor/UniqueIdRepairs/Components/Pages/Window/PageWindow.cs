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

        public void Search (string path) {
            var results = AssetDatabase
                .FindAssets("t:scene", new[] {path})
                .Select(guid => {
                    var fullPath = AssetDatabase.GUIDToAssetPath(guid);
                    return AssetDatabase.LoadAssetAtPath<SceneAsset>(fullPath);
                })
                .ToList();

            SearchScenes(results);
        }

        private void Search () {
            if (!ConfirmSceneChange()) {
                return;
            }

            SearchScenes(GetScenes());
        }

        private void SearchScenes (List<SceneAsset> sceneAssets) {
            var startScenePath = SceneManager.GetActiveScene().path;
            ClearSearchResults();

            var idCounts = new Dictionary<string, int>();
            foreach (var scene in sceneAssets) {
                var sceneSearch = new SceneSearch(_elSearchResults, scene);
                sceneSearch.Records.ForEach(r => {
                    if (!idCounts.ContainsKey(r.id)) idCounts[r.id] = 0;
                    idCounts[r.id] += 1;
                });

                sceneSearch.onFixId += (record) => {
                    idCounts[record.id] -= 1;
                    if (idCounts[record.id] > 1) return;
                    _searchedScenes.ForEach((s) => s.HideId(record.id));
                };

                _searchedScenes.Add(sceneSearch);
            }

            _elSearchText.text = $"Searched {_searchedScenes.Count} scenes";

            _searchedScenes
                .ToList()
                .ForEach(s => {
                    s.PrintDuplicates(idCounts);

                    if (s.ErrorCount == 0) {
                        _searchedScenes.Remove(s);
                        s.Remove();
                    }
                });

            if (_searchedScenes.Count == 0) {
                var elNoErrors = _container.Query<VisualElement>(null, "p-window__no-errors").First();
                elNoErrors.AddToClassList("-show");
            }

            EditorSceneManager.OpenScene(startScenePath, OpenSceneMode.Single);
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
