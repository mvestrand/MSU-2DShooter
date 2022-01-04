using System;
using System.Reflection;

using System.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MVest {


    class ExecuteInEditorAfterAssembly {

        [InitializeOnLoadMethod]
        static void OnAssemblyLoad() {
            HandleAttributes();
        }

        static void HandleAttributes() {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsDefined(typeof(ExecuteInEditorAfterAssemblyAttribute), true) );

            //Debug.LogFormat("{0} types found", types.Count());

            foreach (var type in types) {
                var attributes = type.GetCustomAttributes<ExecuteInEditorAfterAssemblyAttribute>();
                foreach (var attribute in attributes) {
                    if (attribute is CreateUniqueAssetAttribute) {
                        Handle_CreateUniqueAssetAttribute(type, (CreateUniqueAssetAttribute)attribute);
                    }
                }
            }
        }

        static void Handle_CreateUniqueAssetAttribute(Type type, CreateUniqueAssetAttribute attribute) {
            //Debug.LogFormat("Type {0} has CreateUniqueAsset defined", type.FullName);
            if (!typeof(ScriptableObject).IsAssignableFrom(type)) {
                Debug.LogErrorFormat("Attribute CreateUniqueAsset cannot be used on type {0}, it can only be used on ScriptableObjects", type.FullName);
                return;
            }
            string filepath = CleanUpAssetFilepath(attribute.Path, type.Name + ".asset");

            GUID existing = AssetDatabase.GUIDFromAssetPath(filepath);
            if (existing.Empty()) {
                ScriptableObject asset = ScriptableObject.CreateInstance(type);
                if (asset == null)
                    return;
                //Debug.LogFormat("AssetPath: {0}", filepath);
                CreateAssetPath(filepath, false);
                AssetDatabase.CreateAsset(asset, filepath);
            }
        }

        const string assetsPath = "Assets/";

        static string CleanUpAssetFilepath(string filepath, string defaultFileName) {
            // Get a path from whatever garbage we were given
            string newFilepath = String.Join('/', filepath.Split('/')
                .Select(x => x.Trim())
                .Where(x => x.Length > 0));

            // Add assets path if not already included
            if (filepath.Length < assetsPath.Length || !filepath.Substring(0, assetsPath.Length).Equals(assetsPath)) {
                newFilepath = assetsPath + newFilepath;
            }

            // Add the default file name if we weren't given one
            if (!newFilepath.Split('/').Last().Contains('.')) {
                newFilepath += "/" + defaultFileName;
            }
            return newFilepath;
        }

        // This assumes the root of the path is "Assets/" and already exists
        static void CreateAssetPath(string path, bool includeLast = true) {
            if (includeLast)
                CreateAssetPathRecurse(path, null, path);
            else {
                int index = path.LastIndexOf('/');
                string parentPath = path.Substring(0, index);
                CreateAssetPathRecurse(parentPath, null, parentPath);
            }
        }

        static void CreateAssetPathRecurse(string parentPath, string newFolder, string fullPath) {
            //Debug.LogFormat("CreateAssetPath({0}, {1}, {2})", parentPath, newFolder, fullPath);
            // Create parent path if needed
            int index = parentPath.LastIndexOf('/');
            if (index != -1) {
                CreateAssetPathRecurse(parentPath.Substring(0, index), parentPath.Substring(index+1), parentPath);
            }
            if (newFolder != null && !AssetDatabase.IsValidFolder(fullPath)) {
                //Debug.LogFormat("CreateFolder({0}, {1})", parentPath, newFolder);
                AssetDatabase.CreateFolder(parentPath, newFolder);
            }

        }
    }


}
