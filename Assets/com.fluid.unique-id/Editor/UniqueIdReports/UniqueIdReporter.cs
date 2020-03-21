using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CleverCrow.Fluid.UniqueIds {
    public class UniqueIdReporter {
        private readonly string[] _pathScenes;

        public UniqueIdReporter (string path) {
            _pathScenes = AssetDatabase
                .FindAssets("t:scene", new[] {path})
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
        }

        public UniqueIdReporter (IEnumerable<SceneAsset> sceneAssets) {
            _pathScenes = sceneAssets
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();
        }

        public IdReport GetReport () {
            var idCounter = new Dictionary<string, int>();
            var sceneIds = new Dictionary<string, List<ReportId>>();

            foreach (var scene in _pathScenes) {
                if (SceneManager.GetActiveScene().path != scene) {
                    EditorSceneManager.OpenScene(scene, OpenSceneMode.Single);
                }

                var idReports = new List<ReportId>();

                var ids = Object.FindObjectsOfType<UniqueId>().ToList();
                ids.ForEach(id => {
                    var key = id.Id ?? "null";
                    if (!idCounter.ContainsKey(key)) idCounter[key] = 0;
                    idCounter[key] += 1;

                    idReports.Add(new ReportId(id));
                });

                sceneIds[scene] = idReports;
            }

            var errorCount = 0;
            var scenes = new List<ReportScene>();
            foreach (var scene in _pathScenes) {
                var errors = sceneIds[scene]
                    .Where(id => {
                        var isError = id.Id == null || idCounter[id.Id] > 1;
                        if (isError) errorCount += 1;
                        return isError;
                    })
                    .ToList();

                scenes.Add(new ReportScene(scene, errors));
            }

            return new IdReport(errorCount, idCounter, scenes);
        }
    }

    public class IdReport {
        public List<ReportScene> Scenes { get; }
        public int ErrorCount { get; }
        public Dictionary<string, int> DuplicateIDs { get; }

        public IdReport (int errorCount, Dictionary<string, int> duplicateIDs, List<ReportScene> scenes) {
            ErrorCount = errorCount;
            DuplicateIDs = duplicateIDs;
            Scenes = scenes;
        }
    }

    public class ReportScene {
        public List<ReportId> Errors { get; }
        public string Path { get; }

        public ReportScene (string path, List<ReportId> errors) {
            Path = path;
            Errors = errors;
        }
    }

    public class ReportId {
        public string Id { get; }
        public string Name { get; }
        public string Path { get; }

        public ReportId (UniqueId id) {
            Id = id.Id;
            Name = id.name;
            Path = GetPath(id.gameObject);
        }

        public static string GetPath (GameObject id) {
            var pathTarget = id.transform;
            var path = $"{pathTarget.name}";
            while (pathTarget.parent != null) {
                pathTarget = pathTarget.parent;
                path = $"{pathTarget.name}/{path}";
            }

            return path;
        }
    }
}
