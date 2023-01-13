/****************
 *@class name:		EditorLuaUtility
 *@description:		编辑器加载lua的一些公共方法
 *@author:			selik0
 *@date:			2023-01-10 10:45:15
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XLua;
using Pisces;
namespace PiscesEditor
{
    static public class EditorLuaUtility
    {
        static public LuaEnv GetEditorLuaEnv()
        {
            LuaEnv luaEnv = new LuaEnv();
            luaEnv.AddLoader(EditorLuaCustomLoader);
            return luaEnv;
        }

        static byte[] EditorLuaCustomLoader(ref string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MyLogger.LogError("excel2lua", "路径为空");
                return null;
            }
            fileName = fileName.Replace(".", "/");
            foreach (var path in EditorPathUtility.LuaLoaderPaths)
            {
                string luaFilepath = path.Replace("?", fileName);
                if (File.Exists(luaFilepath))
                    return File.ReadAllBytes(luaFilepath);
            }
            MyLogger.LogError("未找到处理excel2lua的lua文件", fileName);
            return null;
        }
    }
}