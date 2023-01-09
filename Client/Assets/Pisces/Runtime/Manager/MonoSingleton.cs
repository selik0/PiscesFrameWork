/****************
 *@class name:		MonoSingleton
 *@description:		
 *@author:			selik0
 *@date:			2023-01-09 14:31:28
 *@version: 		V1.0.0
*************************************************************************/
using UnityEngine;
namespace Pisces
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T m_Instance;
        public static T Instance
        {
            get
            {
                if (!Application.isPlaying) return null;
                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (m_Instance == null)
                {
                    GameObject parent = GameObject.Find("ManagerGo");
                    if (parent == null)
                    {
                        parent = new GameObject("ManagerGo");
                        DontDestroyOnLoad(parent);
                    }
                    GameObject go = new GameObject(typeof(T).Name);
                    m_Instance = go.AddComponent<T>();
                    if (parent != null && go != null)
                        go.transform.parent = parent.transform;
                }
                return m_Instance;
            }
        }
        private void Awake()
        {
            Startup();
        }

        private void OnDestroy()
        {
            Dispose();
        }
        protected abstract void Startup();
        protected abstract void Dispose();
        public abstract void ReStart();
    }
}