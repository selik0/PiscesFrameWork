using System.IO;
/****************
 *@class name:		EditorConfig
 *@description:		
 *@author:			selik0
 *@date:			2022-12-30 10:25:34
 *@version: 		V1.0.0
*************************************************************************/
using UnityEditor;
using UnityEngine;
namespace Pisces
{
    public class EditorConfig : ScriptableObject
    {
        /// <summary>
        /// 图集导出的文件夹位置
        /// </summary>
        private string m_SpriteAtlasExportDirectory = Path.Combine(Application.dataPath, "/MyBuild/SpriteAtlas");
        /// <summary>
        /// excel配置表的根路径
        /// </summary>
        private string m_ExcelConfigRoot = Path.Combine(EditorPathUtility.FrameWorkProjectPath, "ExcelConfig");
        /// <summary>
        /// excel导出的lua文件的保存路径
        /// </summary>
        private string m_ExcelExportDirectory = Path.Combine(Application.dataPath, "/Lua/table/base");
    }
}
