using System.IO;
using System;
/****************
 *@class name:		ExcelConfigReader
 *@description:		
 *@author:			selik0
 *@date:			2023-01-09 22:05:49
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml;
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
            LuaEnv luaEnv = EditorLuaUtility.GetEditorLuaEnv();
            luaEnv.DoString("require('excel2lua')");
            // DirectoryInfo directoryInfo = new DirectoryInfo(EditorPathUtility.ExcelConfigFilePath);
            // FileInfo[] fileInfos = directoryInfo.GetFiles();
            // foreach (var fileInfo in fileInfos)
            // {
            //     if (fileInfo.Name.StartsWith("t_s"))
            //     {
            //         using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            //         {
            //             ExcelWorksheet firstSheet = excelPackage.Workbook.Worksheets[1];
            //             for (int i = 1; i <= firstSheet.Dimension.Columns; i++)
            //             {
            //                 Logger.Log(fileInfo.Name, firstSheet.Cells[9, i].Text);
            //             }
            //         }
            //     }
            // }
        }

        [MenuItem("Tools/Pisces/Excel2Lua")]
        static public void EditorExcel2Lua()
        {
            LuaEnv luaEnv = EditorLuaUtility.GetEditorLuaEnv();
            luaEnv.DoString("require('excel2lua')");
            Func<string, Dictionary<int, Dictionary<int, string>>, string> excle2luaFunc = luaEnv.Global.Get<Func<string, Dictionary<int, Dictionary<int, string>>, string>>("excel2luaFunction");
            if (excle2luaFunc == null)
            {
                Logger.LogError("未找到excel2lua的方法");
                return;
            }
            if (!Directory.Exists(EditorPathUtility.ExcelConfigFilePath))
            {
                Logger.LogError("excel文件未找到", EditorPathUtility.ExcelConfigFilePath);
                return;
            }

            if (!Directory.Exists(EditorPathUtility.ExcelConfigExportDirectory))
                Directory.CreateDirectory(EditorPathUtility.ExcelConfigExportDirectory);

            DirectoryInfo directoryInfo = new DirectoryInfo(EditorPathUtility.ExcelConfigFilePath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach (var fileInfo in fileInfos)
            {
                if (fileInfo.Name.StartsWith("t_s"))
                {
                    using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
                    {
                        ExcelWorksheet firstSheet = excelPackage.Workbook.Worksheets[1];
                        Dictionary<int, Dictionary<int, string>> dic = new Dictionary<int, Dictionary<int, string>>();
                        for (int i = 1; i <= firstSheet.Dimension.Rows; i++)
                        {
                            dic.Add(i, new Dictionary<int, string>());
                            for (int j = 1; j < firstSheet.Dimension.Columns; j++)
                            {
                                dic[i].Add(j, firstSheet.Cells[i, j].Text);
                            }
                        }
                        string luaName = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("-"));
                        string luaScriptName = $"config.{luaName}&config";
                        string content = excle2luaFunc.Invoke(luaScriptName, dic);
                        string luaTablePath = Path.Combine(EditorPathUtility.ExcelConfigExportDirectory, luaName + ".lua");
                        File.WriteAllText(luaTablePath, content);
                    }
                }
            }
            AssetDatabase.Refresh();
        }
    }
}