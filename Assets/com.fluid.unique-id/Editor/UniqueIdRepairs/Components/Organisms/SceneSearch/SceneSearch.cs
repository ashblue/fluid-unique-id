using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    public class SceneSearch : ComponentBase {
        private readonly List<UniqueIdError> _errors = new List<UniqueIdError>();

        public SceneSearch (VisualElement container, ReportScene report, Action<ReportId> onFixId) : base (container) {
            var results = _container.Query<VisualElement>(null, "o-scene-search__results").Last();
            var title = container.Query<TextElement>(null, "o-scene-search__title").Last();
            title.text = report.Path;

            report.Errors.ForEach((error) => {
                _errors.Add(new UniqueIdError(results, report, error, onFixId));
            });
        }

        public void HideId (string id) {
            foreach (var e in _errors.Where(e => e.Id == id)) {
                e.MarkFixed();
            }
        }
    }
}
