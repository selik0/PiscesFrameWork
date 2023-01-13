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
using Pisces;
namespace PiscesEditor
{
    public class EditorConfig : ScriptableObject
    {
        /// <summary>
        /// 图集导出的文件夹位置
        /// </summary>
        public string m_SpriteAtlasExportDirectory = "Assets/MyBuild/SpriteAtlas";
        /// <summary>
        /// excel配置表的根路径
        /// </summary>
        public string m_ExcelConfigRoot = @"H:\PiscesFrameWork\ExcelConfig";
        /// <summary>
        /// excel导出的lua文件的保存路径
        /// </summary>
        public string m_ExcelExportDirectory = "Assets/Lua/table/base";
    }
}
