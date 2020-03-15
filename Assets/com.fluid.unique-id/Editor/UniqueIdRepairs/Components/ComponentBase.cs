using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public abstract class ComponentBase {
        private readonly string _path;
        protected readonly VisualElement _container;

        protected ComponentBase (VisualElement container) {
            _container = container;

            if (_path == null) {
                _path = new System.Diagnostics.StackTrace(true)
                    .GetFrame(1)
                    .GetFileName()
                    ?.Replace("\\", "/")
                    .Replace(Application.dataPath, "Assets")
                    .Replace(".cs", ".uxml");
            }

            var markup = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_path);
            markup.CloneTree(container);
        }
    }
}
