/****************
 *@class name:		MyMenuOption
 *@description:		
 *@author:			selik0
 *@date:			2023-01-09 21:13:42
 *@version: 		V1.0.0
*************************************************************************/
using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
namespace UnityEditor.UI
{
    static class MyMenuOptions
    {
        [MenuItem("GameObject/UI/Scroll View", false, 2062)]
        static public void AddScrollView(MenuCommand menuCommand)
        {
            // GameObject go;
            // using (new FactorySwapToEditor())
            //     go = DefaultControls.CreateScrollView(GetStandardResources());
            // PlaceUIElementRoot(go, menuCommand);
            Debug.Log("ssss");  
        }
    }
}