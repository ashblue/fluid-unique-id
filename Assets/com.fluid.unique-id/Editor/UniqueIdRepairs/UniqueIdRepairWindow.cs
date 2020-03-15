using UnityEditor;
using UnityEngine;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public class UniqueIdRepairWindow : EditorWindow {
        private PageWindow _window;

        [MenuItem("Window/Fluid/Unique ID Repair")]
        public static void ShowWindow () {
            var window = GetWindow<UniqueIdRepairWindow>();
            window.titleContent = new GUIContent("Unique ID Repair");
        }

        private void OnEnable () {
            var root = rootVisualElement;
            _window = new PageWindow(root);
        }
    }
}
