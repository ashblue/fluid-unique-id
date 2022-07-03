using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CleverCrow.Fluid.UniqueIds {
    public class SceneGenerator {
        public string Id { get; }
        public string Path { get; }
        public List<string> ScenePaths { get; } = new List<string>();

        public SceneGenerator (int sameIdCount, int sceneCount = 1, int nullIdCount = 0) {
            var tmpFolder = Guid.NewGuid().ToString();
            AssetDatabase.CreateFolder("Assets", tmpFolder);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            var container = new GameObject("Container").transform;

            UniqueId id = null;
            while (sameIdCount > 0) {
                if (id == null) {
                    var go = new GameObject("UniqueId");
                    go.transform.SetParent(container);
                    id = go.AddComponent<UniqueId>();
                    id.PopulateIdIfEmpty();
                    Id = id.Id;
                } else {
                    Object.Instantiate(id, container);
                }

                sameIdCount -= 1;
            }

            while (nullIdCount > 0) {
                var goNull = new GameObject("UniqueId");
                goNull.transform.SetParent(container);
                goNull.AddComponent<UniqueId>();
                nullIdCount -= 1;
            }

            while (sceneCount != 0) {
                var path = $"Assets/{tmpFolder}/{sceneCount}.unity";
                ScenePaths.Add(path);
                EditorSceneManager.SaveScene(scene, path);
                sceneCount--;
            }

            Path = $"Assets/{tmpFolder}";
        }

        public void Teardown () {
            AssetDatabase.DeleteAsset(Path);
        }
    }
}
