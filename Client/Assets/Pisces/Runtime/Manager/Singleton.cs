/****************
 *@class name:		Singleton
 *@description:		单例的基类
 *@author:			selik0
 *@date:			2023-01-09 11:44:53
 *@version: 		V1.0.0
*************************************************************************/
using System;
namespace Pisces
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T m_Instance;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = Activator.CreateInstance<T>();
                }
                return m_Instance;
            }
        }

        public Singleton()
        {
            Startup();
        }
        public abstract void Startup();
        public abstract void Dispose();
        public abstract void ReStart();
    }
}