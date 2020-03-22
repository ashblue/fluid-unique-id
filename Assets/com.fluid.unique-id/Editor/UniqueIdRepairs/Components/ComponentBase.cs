using System;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public abstract class ComponentBase {
        private readonly string _path;
        protected readonly VisualElement _container;

        protected ComponentBase (VisualElement container) {
            _container = container;

            if (_path == null) {
                var strings = new System.Diagnostics.StackTrace(true)
                    .GetFrame(1)
                    .GetFileName()
                    ?.Replace("\\", "/")
                    .Split(new string[] { "/Assets/" }, StringSplitOptions.None)
                    .ToList();

                _path = $"{AssetPath.BasePath}/{strings[1]}"
                    .Replace(".cs", ".uxml");
            }

            var markup = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_path);
            markup.CloneTree(container);
        }
    }
}
