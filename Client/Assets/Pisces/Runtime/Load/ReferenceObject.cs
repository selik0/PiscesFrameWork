/****************
 *@class name:		ReferenceObject
 *@description:		单个资源的引用计数的类
 *@author:			selik0
 *@date:			2023-01-28 17:12:34
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pisces
{
    public class ReferenceObject<T> where T : UnityEngine.Object
    {
        private T m_Asset = null;
        private int m_ReferencedCount = 0;

        public bool IsNeedRelease()
        {
            return m_ReferencedCount <= 0;
        }

        public void SetAsset(T asset)
        {
            m_Asset = asset;
        }

        public T GetAsset()
        {
            m_ReferencedCount++;
            return m_Asset;
        }

        public void RecycleAsset()
        {
            m_ReferencedCount--;
        }

        public virtual void Release()
        {
            Resources.UnloadAsset(m_Asset);
            m_ReferencedCount = 0;
            m_Asset = null;
        }
    }
}