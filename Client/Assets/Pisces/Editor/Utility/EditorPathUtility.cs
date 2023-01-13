using System.IO;
/****************
 *@class name:		EditorPathConfig
 *@description:		一些编辑器需要用的路径
 *@author:			selik0
 *@date:			2022-12-29 21:46:28
 *@version: 		V1.0.0
*************************************************************************/
using UnityEditor;
using UnityEngine;
using Pisces;
namespace PiscesEditor
{
    public class EditorPathUtility
    {
        /// <summary>
        /// unity客户端最外层的绝对路径，示例C:/FrameWork/Client
        /// </summary>
        /// <returns></returns>
        static public string ClientProjectPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        /// <summary>
        /// 框架最外层的绝对路径，示例C:/FrameWork
        /// </summary>
        /// <returns></returns>
        static public string FrameWorkProjectPath = ClientProjectPath.Substring(0, ClientProjectPath.LastIndexOf("/"));
        /// <summary>
        /// 框架的excel配置表根路径
        /// </summary>
        /// <returns></returns>
        static public string FrameWorkExcelRootPath = Path.Combine(FrameWorkProjectPath, "ExcelConfig");
        /// <summary>
        /// 框架的excel配置表路径
        /// </summary>
        /// <returns></returns>
        static public string ExcelConfigFilePath = Path.Combine(FrameWorkExcelRootPath, "Excel");
        /// <summary>
        /// 单个excle的lua配置表。通过这个lua转换数据的格式和检查数据
        /// </summary>
        /// <returns></returns>
        static public string[] LuaLoaderPaths = new string[]{
            Path.Combine(FrameWorkExcelRootPath, "Excel2Lua", "?.lua"),
            Path.Combine(Application.dataPath, "Lua", "?.lua"),
            Path.Combine(Application.dataPath, "Pisces", "Runtime", "Lua", "?.lua"),
        };
        /// <summary>
        /// 图集导出的文件夹位置
        /// </summary>
        static public string SpriteAtlasExportDirectory = Path.Combine(Application.dataPath, "/MyBuild/SpriteAtlas");
        /// <summary>
        /// excel导出的lua文件的保存路径
        /// </summary>
        static public string ExcelConfigExportDirectory = Path.Combine(Application.dataPath, "Lua", "table", "base");

        static public string UIPrefabsSavePath = Path.Combine(Application.dataPath, "MyAssets", "UI", "Prefabs");
    }
}
