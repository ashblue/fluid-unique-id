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

        public SceneGenerator (int sameIdCount, int sceneCount = 1) {
            var tmpFolder = Guid.NewGuid().ToString();
            AssetDatabase.CreateFolder("Assets", tmpFolder);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            var container = new GameObject("Container").transform;

            var go = new GameObject("UniqueId");
            go.transform.SetParent(container);
            var id = go.AddComponent<UniqueId>();
            id.PopulateIdIfEmpty();
            Id = id.Id;
            sameIdCount -= 1;

            while (sameIdCount > 0) {
                Object.Instantiate(id, container);
                sameIdCount -= 1;
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
