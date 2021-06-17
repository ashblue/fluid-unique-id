using UnityEditor;
using UnityEngine;

namespace CleverCrow.Fluid.UniqueIds {
    public class UniqueIdEditorEvents {
        [InitializeOnLoad]
        class UniqueIdPrefabApplyCallback {
            static UniqueIdPrefabApplyCallback () {
                PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdate;
            }

            static void OnPrefabInstanceUpdate (GameObject instance) {
                var id = instance.GetComponent<UniqueId>();
                if (id == null) return;

                var prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(instance);
                if (prefabParent == null) return;

                var parentId = prefabParent.GetComponent<UniqueId>();
                if (parentId == null) return;

                var idSerializedObject = new SerializedObject(id);
                var idProp = idSerializedObject.FindProperty("_id");

                var parentIdSerializedObject = new SerializedObject(parentId);
                var parentIdProp = parentIdSerializedObject.FindProperty("_id");

                if (idProp.stringValue == parentIdProp.stringValue) {
                    var prevId = idProp.stringValue;
                    parentIdProp.stringValue = null;
                    parentIdSerializedObject.ApplyModifiedProperties();

                    idProp.stringValue = prevId;
                    idSerializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
