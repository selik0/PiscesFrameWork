using System.IO;
/****************
 *@class name:		EditorConfigManager
 *@description:		编辑器非运行模式下的框架的配置文件管理
 *@author:			selik0
 *@date:			2023-01-13 17:10:24
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pisces;
namespace PiscesEditor
{
    static public class EditorConfigManager
    {
        static public EditorConfig EditorConfig;
        static public PiscesConfig PiscesConfig;

        [ExecuteInEditMode]
        [InitializeOnLoadMethod]
        static public void LoadPisceConfig()
        {
            EditorConfig = EditorAssetUtility.GetOrCreateScriptableObject<EditorConfig>("Assets/Pisces/Editor/EditorConfig.asset");
            PiscesConfig = EditorAssetUtility.GetOrCreateScriptableObject<PiscesConfig>("Assets/Pisces/Runtime/PiscesConfig.asset");
        }
    }
}