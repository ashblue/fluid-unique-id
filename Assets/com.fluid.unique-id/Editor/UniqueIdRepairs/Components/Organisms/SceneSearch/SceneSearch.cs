using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public class SceneSearch : ComponentBase {
        private readonly List<UniqueIdError> _errors = new List<UniqueIdError>();
        private readonly VisualElement _results;
        private readonly VisualElement _root;
        public Action<UniqueIdRecord> onFixId;

        public List<UniqueIdRecord> Records { get; } = new List<UniqueIdRecord>();
        public int ErrorCount => _errors.Count;

        public class UniqueIdRecord {
            public string name;
            public string id;
            public SceneAsset scene;
        }

        public SceneSearch (VisualElement container, SceneAsset scene) : base (container) {
            var path = AssetDatabase.GetAssetPath(scene);
            _root = _container.Query<VisualElement>(null, "o-scene-search").Last().parent;
            _results = _container.Query<VisualElement>(null, "o-scene-search__results").Last();
            var title = container.Query<TextElement>(null, "o-scene-search__title").Last();
            title.text = path;

            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            Object.FindObjectsOfType<UniqueId>().ToList().ForEach((id) => {
                Records.Add(new UniqueIdRecord {
                    name = id.name,
                    id = id.Id,
                    scene = scene,
                });
            });
        }

        public void PrintDuplicates (Dictionary<string, int> idCounts) {
            foreach (var record in Records) {
                if (idCounts[record.id] <= 1) continue;
                _errors.Add(new UniqueIdError(_results, record, onFixId));
            }
        }

        public void Remove () {
            _container.Remove(_root);
        }

        public void HideId (string id) {
            foreach (var e in _errors.Where(e => e.Id == id)) {
                e.MarkFixed();
            }
        }
    }
}
