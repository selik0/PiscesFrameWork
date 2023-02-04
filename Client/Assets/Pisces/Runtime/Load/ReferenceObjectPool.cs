/****************
 *@class name:		ReferenceObjectPool
 *@description:		资源对象池
 *@author:			selik0
 *@date:			2023-01-28 17:49:04
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pisces
{
    public class ReferenceObjectPool<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// 通过资源路径获取资源对象
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, ReferenceObject<T>> m_AssetPathDict = new Dictionary<string, ReferenceObject<T>>();
        /// <summary>
        /// 等待回收的资源队列
        /// </summary>
        /// <returns></returns>
        protected Queue<ReferenceObject<T>> m_WaitReleaseQueue = new Queue<ReferenceObject<T>>(10);

        public void Dispose()
        {
            foreach (var item in m_AssetPathDict.Values)
                m_WaitReleaseQueue.Enqueue(item);
            m_AssetPathDict.Clear();
        }

        public void Add(string key, T asset)
        {
            if (string.IsNullOrEmpty(key))
                return;
            if (!m_AssetPathDict.ContainsKey(key))
            {
                var referencedObj = new ReferenceObject<T>();
                referencedObj.SetAsset(asset);
                m_AssetPathDict.Add(key, referencedObj);
            }
        }

        public T GetAsset(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            if (m_AssetPathDict.ContainsKey(key))
                return m_AssetPathDict[key].GetAsset();
            return null;
        }

        public void RecycleAsset(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;
            if (m_AssetPathDict.ContainsKey(key))
            {
                var temp = m_AssetPathDict[key];
                temp.RecycleAsset();
                if (temp.IsNeedRelease())
                    m_AssetPathDict.Remove(key);
            }
        }
    }
}