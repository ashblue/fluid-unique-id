using System;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public class Button : ComponentBase {
        public Button (VisualElement container, string text, Action callback) : base(container) {
            var el = container.Query<UnityEngine.UIElements.Button>(null, "a-button").First();
            el.text = text;
            el.clicked += callback;
        }
    }
}
