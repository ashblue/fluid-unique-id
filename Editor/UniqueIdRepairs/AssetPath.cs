using UnityEditor;
using UnityEngine;

namespace CleverCrow.Fluid.UniqueIds.UniqueIdRepairs {
    /// <summary>
    /// Determine if this is a package of the Unity Editor since Unity has no API to determine this
    /// </summary>
    public static class AssetPath {
        private const string PATH_PROJECT = "Assets/com.fluid.unique-id";
        private const string PATH_PACKAGE = "Packages/com.fluid.unique-id";

        private static string _basePath;

        public static string BasePath {
            get {
                if (_basePath != null) return _basePath;

                if (AssetDatabase.IsValidFolder(PATH_PACKAGE)) {
                    _basePath = "Packages/";
                    return _basePath;
                }

                if (AssetDatabase.IsValidFolder(PATH_PROJECT)) {
                    _basePath = "Assets/";
                    return _basePath;
                }

                Debug.LogError("Asset root could not be found");

                return null;
            }
        }
    }
}
