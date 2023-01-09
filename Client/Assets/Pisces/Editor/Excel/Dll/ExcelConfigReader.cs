/****************
 *@class name:		ExcelConfigReader
 *@description:		
 *@author:			selik0
 *@date:			2023-01-09 22:05:49
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XLua;
namespace Pisces
{
    public class ExcelConfigReader
    {
        [MenuItem("Tools/Pisces/Test")]
        static public void EditorTest()
        {
            LuaEnv luaEnv = new LuaEnv();
            luaEnv.DoString("print('123')");
        }
    }
}