using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CleverCrow.Fluid.UniqueIds {
    public class UniqueIdReporter {
        public string Path { get; }

        public UniqueIdReporter (string path) {
            Path = path;
        }

        public IdReport GetReport () {
            var idCounter = new Dictionary<string, int>();
            var ids = Object.FindObjectsOfType<UniqueId>().ToList();

            ids.ForEach(id => {
                if (!idCounter.ContainsKey(id.Id)) idCounter[id.Id] = 0;
                idCounter[id.Id] += 1;
            });

            var errorCount = 0;
            var errors = ids
                .Where(id => {
                    var isError = idCounter[id.Id] > 1;
                    if (isError) errorCount += 1;
                    return isError;
                })
                .Select(id => new ReportError(id))
                .ToList();

            var scenes = new List<ReportScene> {
                new ReportScene(errors)
            };

            return new IdReport(errorCount, scenes);
        }
    }

    public class IdReport {
        public List<ReportScene> Scenes { get; }
        public int ErrorCount { get; }

        public IdReport (int errorCount, List<ReportScene> scenes) {
            ErrorCount = errorCount;
            Scenes = scenes;
        }
    }

    public class ReportScene {
        public List<ReportError> Errors { get; }

        public ReportScene (List<ReportError> errors) {
            Errors = errors;
        }
    }

    public class ReportError {
        public string Id { get; }
        public string Name { get; }

        public ReportError (UniqueId id) {
            Id = id.Id;
            Name = id.name;
        }
    }
}
