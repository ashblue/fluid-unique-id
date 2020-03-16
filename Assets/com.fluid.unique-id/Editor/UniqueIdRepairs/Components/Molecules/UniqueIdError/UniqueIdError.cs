using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public class UniqueIdError : ComponentBase {
        private readonly SceneSearch.UniqueIdRecord _record;
        private readonly Action<SceneSearch.UniqueIdRecord> _onFixId;
        private readonly VisualElement _root;

        public string Id { get; }

        public UniqueIdError (
            VisualElement container,
            SceneSearch.UniqueIdRecord record,
            Action<SceneSearch.UniqueIdRecord> onFixId) : base(container) {
            _record = record;
            _onFixId = onFixId;
            Id = _record.id;

            _root = container.Query<VisualElement>(null, "m-unique-id-error").Last();
            var elText = container.Query<TextElement>(null, "m-unique-id-error__text").Last();
            elText.text = $"{record.id}: {record.name}";

            container
                .Query<UnityEngine.UIElements.Button>(null, "m-unique-id-error__fix")
                .Last()
                .clicked += FixId;
        }


        private void FixId () {
            var path = AssetDatabase.GetAssetPath(_record.scene);
            var prevScene = SceneManager.GetActiveScene();
            if (path != prevScene.path && !ConfirmSceneChange()) return;

            if (path != prevScene.path) {
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            }

            var result = Object
                .FindObjectsOfType<UniqueId>()
                .ToList()
                .Find(uniqueId => {
                    return uniqueId.name == _record.name && uniqueId.Id == _record.id;
                });

            Debug.Assert(result != null, "Failed to fix UniqueId");
            var obj = new SerializedObject(result);
            var idProp = obj.FindProperty("_id");
            idProp.stringValue = Guid.NewGuid().ToString();
            obj.ApplyModifiedProperties();

            EditorSceneManager.SaveOpenScenes();

            MarkFixed();
            _onFixId.Invoke(_record);
        }

        private bool ConfirmSceneChange () {
            return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        public void MarkFixed () {
            _root.AddToClassList("-fixed");
        }
    }
}
