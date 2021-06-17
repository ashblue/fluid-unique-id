using System;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace CleverCrow.Fluid.UniqueIds {
    [CustomEditor(typeof(UniqueId), true)]
    public class UniqueIdEditor : Editor {
        bool IsPrefabInstance => PrefabUtility.GetPrefabInstanceStatus(target) != PrefabInstanceStatus.NotAPrefab;
        private bool IsPrefabStage => PrefabStageUtility.GetCurrentPrefabStage() != null;
        bool IsPrefab => PrefabUtility.GetPrefabAssetType(target) != PrefabAssetType.NotAPrefab && !IsPrefabInstance || IsPrefabStage;

        public override void OnInspectorGUI () {
            base.OnInspectorGUI();

            var idProp = serializedObject.FindProperty("_id");
            var id = idProp.stringValue;

            GUI.enabled = false;
            EditorGUILayout.TextField("id", id);
            GUI.enabled = true;

            if (!IsPrefab) {
                EditForm(idProp);
            }

            if (IsPrefab) {
                if (!string.IsNullOrEmpty(id)) {
                    idProp.stringValue = null;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void EditForm (SerializedProperty idProp) {
            if (string.IsNullOrEmpty(idProp.stringValue)) {
                idProp.stringValue = GetGUID();
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Copy ID")) {
                var textEditor = new TextEditor {text = idProp.stringValue};
                textEditor.SelectAll();
                textEditor.Copy();
            }

            if (GUILayout.Button("Randomize ID")
                && EditorUtility.DisplayDialog("Randomize ID", "Are you sure, this can break existing save data", "Randomize",
                    "Cancel")) {
                idProp.stringValue = GetGUID();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static string GetGUID () {
            return Guid.NewGuid().ToString();
        }
    }
}
