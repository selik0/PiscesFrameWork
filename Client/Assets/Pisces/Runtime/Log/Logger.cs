using System.Text;
/****************
 *@class name:		Logger
 *@description:		自定义日志类
 *@author:			selik0
 *@date:			2023-01-07 20:35:30
 *@version: 		V1.0.0
*************************************************************************/
using UnityEngine;
namespace Pisces
{
    public class Logger
    {
        public static void Log(string msg)
        {
            Debug.Log(msg);
        }
        public static void LogWarning(string msg)
        {
            Debug.LogWarning(msg);
        }
        public static void LogError(string msg)
        {
            Debug.LogError(msg);
        }

        static string GetMsg(params object[] msg)
        {
            if (msg == null || msg.Length <= 0)
                return "";
            StringBuilder content = new StringBuilder();
            foreach (var item in msg)
            {
                content.Append(item);
                content.Append("\t");
            }
            return content.ToString();
        }
        public static void Log(params string[] msg)
        {
            Debug.Log(GetMsg(msg));
        }
        public static void LogWarning(params string[] msg)
        {
            Debug.LogWarning(GetMsg(msg));
        }
        public static void LogError(params string[] msg)
        {
            Debug.LogError(GetMsg(msg));
        }
    }
}