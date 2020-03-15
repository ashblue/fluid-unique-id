using UnityEngine.UIElements;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public class UniqueIdError : ComponentBase {
        public UniqueIdError (VisualElement container, string text) : base(container) {
            var elText = container.Query<TextElement>(null, "m-unique-id-error__text").Last();
            elText.text = text;
        }
    }
}
