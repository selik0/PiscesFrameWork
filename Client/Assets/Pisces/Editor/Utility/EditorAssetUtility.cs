
using System.IO;
using System;
/****************
 *@class name:		EditorAssetUtility
 *@description:		
 *@author:			selik0
 *@date:			2023-01-07 15:28:43
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Pisces
{
    public class EditorAssetUtility
    {
        public static string[] GetAssetPaths<T>(string[] searchInFolders = null) where T : UnityEngine.Object
        {
            T t = default(T);
            string[] guids = null;
            string[] paths = null;

            string typeName = $"t:{t.GetType().Name}";

            if (searchInFolders == null)
                guids = AssetDatabase.FindAssets(typeName);
            else
                guids = AssetDatabase.FindAssets(typeName, searchInFolders);

            if (guids == null || guids.Length <= 0)
                return paths;

            paths = new string[guids.Length];

            for (var i = 0; i < guids.Length; i++)
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

            return paths;
        }

        public static T[] GetAssets<T>(string[] searchInFolders = null) where T : UnityEngine.Object
        {
            T[] results = null;
            string[] paths = GetAssetPaths<T>(searchInFolders);

            if (paths == null || paths.Length <= 0)
                return results;

            results = new T[paths.Length];
            for (var i = 0; i < paths.Length; i++)
            {
                results[i] = AssetDatabase.LoadAssetAtPath<T>(paths[i]);
            }
            return results;
        }

        public static T GetAsset<T>(string[] searchInFolders = null) where T : UnityEngine.Object
        {
            T result = null;
            string[] paths = GetAssetPaths<T>(searchInFolders);

            if (paths == null || paths.Length <= 0)
                return result;

            result = AssetDatabase.LoadAssetAtPath<T>(paths[0]);

            return result;
        }

        public static T GetOrCreateScriptableObject<T>(string savePath = "", string[] searchInFolders = null) where T : UnityEngine.ScriptableObject
        {
            T result = null;
            string[] paths = GetAssetPaths<T>(searchInFolders);

            if (paths == null || paths.Length <= 0)
            {
                if (string.IsNullOrEmpty(savePath)) 
                    savePath = Path.Combine(Application.dataPath, $"{result.GetType().Name}.asset");
                result = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(result, savePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return result;
            }
            else
            {
                result = AssetDatabase.LoadAssetAtPath<T>(paths[0]);
                return result;
            }
        }
    }
}
