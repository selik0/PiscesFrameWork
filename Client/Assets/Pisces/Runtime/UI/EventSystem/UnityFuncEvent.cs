/****************
 *@class name:		UnityFuncEvent
 *@description:		
 *@author:			selik0
 *@date:			2023-02-22 15:30:33
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
namespace UnityEngine.Events
{
    public abstract class UnityFuncEvent<T1, T2>
    {
        // private delegate T2 MyFunc(T1 t1);
        // private event MyFunc m_MyFunc;
        private event System.Func<T1, T2> m_MyFunc;

        public void AddListener(System.Func<T1, T2> func)
        {
            m_MyFunc += func;
        }

        public void RemoveListener(System.Func<T1, T2> func)
        {
            m_MyFunc -= func;
        }

        public void RemoveAllListeners()
        {
            m_MyFunc = null;
        }

        public bool IsEmpty()
        {
            return m_MyFunc == null;
        }

        public T2 Invoke(T1 t1)
        {
            if (null == m_MyFunc)
                return default(T2);
            return m_MyFunc.Invoke(t1);
        }
    }
}