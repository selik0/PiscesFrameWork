using System.Net.Mime;
/****************
 *@class name:		PathUtility
 *@description:		路径的一些方法和一些基本路径配置
 *@author:			selik0
 *@date:			2023-01-07 17:10:01
 *@version: 		V1.0.0
*************************************************************************/
using UnityEngine;
using System.IO;
namespace Pisces
{
    public class PathUtility
    {
        public static string WriteablePathRoot
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        return Application.persistentDataPath;
                    case RuntimePlatform.IPhonePlayer:
                        return Application.persistentDataPath;
                    case RuntimePlatform.WindowsEditor:
                        return Path.Combine(Application.dataPath, "..", "files");
                    case RuntimePlatform.OSXEditor:
                        return Path.Combine(Application.dataPath, "..", "files");
                    default:
                        Logger.LogError("Check The WriteablePath");
                        break;
                }
                return Application.persistentDataPath;
            }
        }
    }
}