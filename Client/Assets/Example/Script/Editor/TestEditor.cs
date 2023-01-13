using System.Diagnostics;
/****************
 *@class name:		TestEditor
 *@description:		
 *@author:			selik0
 *@date:			2023-01-08 15:32:36
 *@version: 		V1.0.0
*************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;
namespace Pisces
{
    public class TestEditor
    {
        [MenuItem("Tools/SomeTest")]
        public static void TestSome()
        {
            List<string> namespaces = new List<string>() // 在这里添加名字空间
               {
                   "UnityEngine",
                   "UnityEngine.UI"
               };
            var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                              where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                              from type in assembly.GetExportedTypes()
                              where type.Namespace != null && namespaces.Contains(type.Namespace)
                                      && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                              select type);

            foreach (var item in unityTypes)
            {
                MyLogger.Log(item.Namespace +"."+ item.Name);
            }
        }
    }
}