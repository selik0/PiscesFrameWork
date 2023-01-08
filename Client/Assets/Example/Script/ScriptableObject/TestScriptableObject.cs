using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pisces;
public class TestScriptableObject : MonoBehaviour
{

    /// <summary>
    /// 框架最外层的绝对路径，示例C:/FrameWork
    /// </summary>
    /// <returns></returns>

    private void Start()
    {
        string ClientProjectPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        string FrameWorkProjectPath = ClientProjectPath.Substring(0, ClientProjectPath.LastIndexOf("/"));
        Debug.Log(ClientProjectPath);
        Debug.Log(FrameWorkProjectPath);
    }
}
