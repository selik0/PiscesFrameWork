/****************
 *@class name:		LuaGenConfig
 *@description:		lua需要生产的C#代码配置
 *@author:			selik0
 *@date:			2023-01-07 17:26:05
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;
namespace Pisces
{
    public static class LuaGenConfig
    {
        // [LuaCallCSharp]
        // public static IEnumerable<Type> LuaCallCSharp
        // {
        //     get
        //     {
        //         List<string> namespaces = new List<string>() // 在这里添加名字空间
        //        {
        //            "UnityEngine",
        //            "UnityEngine.UI"
        //        };
        //         var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
        //                           where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
        //                           from type in assembly.GetExportedTypes()
        //                           where type.Namespace != null && namespaces.Contains(type.Namespace) && !isExcluded(type)
        //                                   && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
        //                           select type);

        //         string[] customAssemblys = new string[] {
        //            "Assembly-CSharp",
        //        };
        //         var customTypes = (from assembly in customAssemblys.Select(s => Assembly.Load(s))
        //                            from type in assembly.GetExportedTypes()
        //                            where type.Namespace == null || !type.Namespace.StartsWith("XLua")
        //                                    && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
        //                            select type);
        //         return unityTypes.Concat(customTypes);
        //     }
        // }
 
        //黑名单
        [BlackList]
        public static List<List<string>> BlackList = new List<List<string>>()  {
		    // unity
		    new List<string>(){"UnityEngine.WWW", "movie"},
            new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
            new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
            new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
            new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
            new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
            new List<string>(){"UnityEngine.Light", "areaSize"},
            new List<string>(){"UnityEngine.Light", "SetLightDirty"},
            new List<string>(){"UnityEngine.Light", "shadowRadius"},
            new List<string>(){"UnityEngine.Light", "shadowAngle"},
            new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
#if !UNITY_WEBPLAYER
		    new List<string>(){"UnityEngine.Application", "ExternalEval"},
#endif
		    new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
		    new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
		    new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
            new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
            new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
            new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
            new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
            new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
            new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
            new List<string>(){"UnityEngine.UI.Text", "OnRebuildRequested"},
            new List<string>(){"UnityEngine.Input","IsJoystickPreconfigured","System.String"},
        };
    }
}