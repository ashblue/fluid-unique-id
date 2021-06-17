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
        private readonly ReportId _report;
        private readonly Action<ReportId> _onFixId;
        private readonly VisualElement _root;
        private readonly ReportScene _scene;
        private readonly TextElement _elId;

        public string Id { get; }

        public UniqueIdError (
            VisualElement container,
            ReportScene scene,
            ReportId report,
            Action<ReportId> onFixId) : base(container) {
            _report = report;
            _onFixId = onFixId;
            _scene = scene;
            Id = _report.Id;

            _root = container.Query<VisualElement>(null, "m-unique-id-error").Last();
            _elId = container.Query<TextElement>(null, "m-unique-id-error__text").Last();
            var printText = string.IsNullOrEmpty(_report.Id) ? "null" : _report.Id;
            _elId.text = printText;

            var elName = container.Query<TextElement>(null, "m-unique-id-error__name").Last();
            elName.text = _report.Path;

            container
                .Query<UnityEngine.UIElements.Button>(null, "m-unique-id-error__fix")
                .Last()
                .clicked += FixId;

            container
                .Query<UnityEngine.UIElements.Button>(null, "m-unique-id-error__show")
                .Last()
                .clicked += ShowId;
        }

        private void ShowId () {
            if (!ShowScene()) return;

            var result = FindId();
            Selection.activeObject = result.gameObject;
        }

        private void FixId () {
            if (!ShowScene()) return;

            var result = FindId();
            var obj = new SerializedObject(result);
            var idProp = obj.FindProperty("_id");
            idProp.stringValue = Guid.NewGuid().ToString();
            obj.ApplyModifiedProperties();

            EditorSceneManager.SaveOpenScenes();

            MarkFixed();
            _onFixId.Invoke(_report);
            _elId.text = idProp.stringValue;
        }

        private UniqueId FindId () {
            var result = Object
                .FindObjectsOfType<UniqueId>()
                .ToList()
                .Find(uniqueId => {
                    var path = ReportId.GetPath(uniqueId.gameObject);
                    return path == _report.Path;
                });

            Debug.Assert(result != null, "Failed to find UniqueId GameObject");

            return result;
        }

        private bool ShowScene () {
            var prevScene = SceneManager.GetActiveScene();
            if (_scene.Path != prevScene.path && !ConfirmSceneChange()) return false;

            if (_scene.Path != prevScene.path) {
                EditorSceneManager.OpenScene(_scene.Path, OpenSceneMode.Single);
            }

            return true;
        }

        private bool ConfirmSceneChange () {
            return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        public void MarkFixed () {
            _root.AddToClassList("-fixed");
        }
    }
}
