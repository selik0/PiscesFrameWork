/****************
 *@class name:		EditorPathConfig
 *@description:		一些编辑器需要用的路径
 *@author:			selik0
 *@date:			2022-12-29 21:46:28
 *@version: 		V1.0.0
*************************************************************************/
using UnityEditor;
using UnityEngine;
namespace Pisces
{
    public class EditorPathUtility
    {
        /// <summary>
        /// unity客户端最外层的绝对路径，示例C:/FrameWork/Client
        /// </summary>
        /// <returns></returns>
        public static string ClientProjectPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        /// <summary>
        /// 框架最外层的绝对路径，示例C:/FrameWork
        /// </summary>
        /// <returns></returns>
        public static string FrameWorkProjectPath = ClientProjectPath.Substring(0, ClientProjectPath.LastIndexOf("/"));
    }
}
