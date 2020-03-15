using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public class SceneSearch : ComponentBase {
        private List<UniqueIdError> _errors = new List<UniqueIdError>();
        private readonly VisualElement _results;
        public List<UniqueIdRecord> Records { get; } = new List<UniqueIdRecord>();

        public class UniqueIdRecord {
            public string name;
            public string id;
        }

        public SceneSearch (VisualElement container, SceneAsset scene) : base (container) {
            var path = AssetDatabase.GetAssetPath(scene);
            _results = _container.Query<VisualElement>(null, "o-scene-search__results").Last();
            var title = container.Query<TextElement>(null, "o-scene-search__title").Last();
            title.text = path;

            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            Object.FindObjectsOfType<UniqueId>().ToList().ForEach((id) => {
                Records.Add(new UniqueIdRecord {
                    name = id.name,
                    id = id.Id
                });
            });
        }

        public void PrintDuplicates (Dictionary<string, int> idCounts) {
            foreach (var record in Records) {
                if (idCounts[record.id] <= 1) continue;
                _errors.Add(new UniqueIdError(_results, $"{record.name}: {record.id}"));
            }
        }
    }
}
