/****************
 *@class name:		AbstractScrollGrid
 *@description:		grid抽象类，
 可以实现固定大小的Scroll,
 动态大小的ScrollGrid,
 动态ElementPrefab的ScrollGird,
 *@author:			selik0
 *@date:			2023-02-07 21:56:41
 *@version: 		V1.0.0
*************************************************************************/
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Collections;
namespace UnityEngine.UI
{
    [RequireComponent(typeof(Scroll))]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class AbstractScrollGrid : UIBehaviour
    {
        [Serializable]
        /// <summary>
        /// scrollGrid元素改变时的回调
        /// </summary>
        public class ScrollGridEvent : UnityEvent<int, GameObject> { }

        #region  Inspector属性
        [SerializeField]
        private ScrollGridEvent m_OnElementChange = new ScrollGridEvent();
        /// <summary>
        /// Grid Cell从新设置的回调
        /// </summary>
        public ScrollGridEvent onElementChange
        {
            get { return m_OnElementChange; }
            set { m_OnElementChange = value; }
        }

        [SerializeField]
        protected List<GameObject> m_ElementPrefabs;

        public int headPadding = 20;
        public int tailPadding = 20;

        [Tooltip("一行或一列中element的数量")]
        [Range(1, 10)]
        public int groupElementCount = 1;

        [Tooltip("Element Prefab的大小")]
        public List<Vector2> elementSizes = new List<Vector2>();

        [Tooltip("Element Prefab之间的间隔")]
        public Vector2 elementSpacing = new Vector2(20, 20);
        #endregion

        private Scroll m_Scroll;
        public Scroll scroll
        {
            get
            {
                if (null == m_Scroll)
                    m_Scroll = GetComponent<Scroll>();
                return m_Scroll;
            }
        }
        /// <summary>
        /// ScrollGrid的滑动方向
        /// </summary>
        public Scroll.Direction direction => scroll.direction;

        protected int m_Count;

        protected int m_Axis => (int)direction;
        // 当前的最小element的下标
        protected int m_CurrentDisplayElementIndex = 0;

        protected Vector2 m_OldNormalize;
        protected Vector2 m_OldScrollPosition;
        protected Vector2 m_OldContentSize;
        /// <summary>
        /// 缓存创建的ElementPrefab
        /// </summary>
        protected Dictionary<int, Queue<GameObject>> m_CacheElementDict = new Dictionary<int, Queue<GameObject>>();
        /// <summary>
        /// 缓存显示的cell数据
        /// </summary>
        protected Dictionary<int, ScrollGridCell> m_DisplayElementDict = new Dictionary<int, ScrollGridCell>();
        /// <summary>
        /// 缓存所有的cell数据
        /// </summary>
        protected Dictionary<int, ScrollGridCell> m_TotalElementDict = new Dictionary<int, ScrollGridCell>();

        protected override void Awake()
        {
            foreach (var go in m_ElementPrefabs)
                go?.SetActive(false);

            scroll.onValueChanged.AddListener(OnScroll);
        }

        protected override void OnDestroy()
        {
            scroll.onValueChanged.RemoveListener(OnScroll);
        }

        /// <summary>
        /// 根据下标获取对应的Cell数据,未获取到则创建
        /// </summary>
        /// <param name="index">下标</param>
        /// <param name="cell">cell数据类</param>
        /// <returns>是否获取到</returns>
        protected virtual bool GetAndCreateCell(int index, out ScrollGridCell cell)
        {
            cell = null;
            if (index < 0 || index > m_Count)
                return false;

            if (GetCell(index, out cell))
                return true;

            cell = new ScrollGridCell(index);
            cell.goIndex = GetElementIndexByIndex(cell.index);
            cell.nowSize = elementSizes[cell.goIndex];
            m_TotalElementDict.Add(index, cell);
            return true;
        }

        protected bool GetCell(int index, out ScrollGridCell cell)
        {
            if (m_TotalElementDict.TryGetValue(index, out cell))
                return true;
            return false;
        }

        /// <summary>
        /// 显示单个Element Prefab
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected abstract bool DisplayCell(int index);


        protected abstract void DisplayCells(int beginIndex, bool isAdd);


        /// <summary>
        /// 根据下标选择elementPrefab
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>elementPrefabs的下标</returns>
        protected abstract int GetElementIndexByIndex(int index);

        protected abstract void UpdateCellPosition(ScrollGridCell cell);

        protected abstract void UpdateContentSize();

        /// <summary>
        /// 根据下标获取实例化的Element
        /// </summary>
        /// <param name="index">grid cell的下标</param>
        /// <returns>实例化的element</returns>
        protected bool ElementDeCacheQueue(ScrollGridCell cell)
        {
            if (null != cell.go)
                return false;

            GameObject go = null;
            Queue<GameObject> queue = null;

            // 从缓存里拿
            if (m_CacheElementDict.TryGetValue(cell.goIndex, out queue) && queue.Count > 0)
            {
                cell.SetTransform(queue.Dequeue());
                return true;
            }

            // 创建一个新的Go
            if (null == go && m_ElementPrefabs.Count > cell.goIndex && null != m_ElementPrefabs[cell.goIndex])
            {
                cell.SetTransform(Instantiate<GameObject>(m_ElementPrefabs[cell.goIndex]));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="cell"></param>
        protected virtual void ElementEnCacheQueue(ScrollGridCell cell)
        {
            if (null == cell.go)
                return;

            Queue<GameObject> queue = null;
            if (!m_CacheElementDict.TryGetValue(cell.goIndex, out queue))
            {
                queue = new Queue<GameObject>();
                m_CacheElementDict.Add(cell.goIndex, queue);
            }
            cell.go.SetActive(false);
            queue.Enqueue(cell.go);
            cell.go = null;
        }

        /// <summary>
        /// 刷新grid中element的数量
        /// </summary>
        /// <param name="count">最新的数量</param>
        /// <param name="isReset">是否重置cell数据</param>
        public virtual void UpdateCount(int count, bool isReset = true, bool isScrollToHead = true)
        {
            // 先停止scroll的惯性滑动
            scroll.StopMovement();

            if (count <= 0)
                return;

            m_Count = count;

            // 是否重置cell的数据
            if (isReset)
            {
                // 先回收所有显示
                foreach (var cell in m_TotalElementDict.Values)
                {
                    ElementEnCacheQueue(cell);
                }
            }

            // 数量多了则删除多余的cell数据
            if (m_TotalElementDict.Count > m_Count)
            {
                for (int i = m_TotalElementDict.Count - 1; i >= m_Count; i--)
                {
                    if (m_DisplayElementDict.ContainsKey(i))
                    {
                        ElementEnCacheQueue(m_DisplayElementDict[i]);
                        m_DisplayElementDict.Remove(i);
                    }
                    m_TotalElementDict.Remove(i);
                }
            }
            else if (m_TotalElementDict.Count < m_Count)
            {
                DisplayCells(m_TotalElementDict.Count, true);
            }
        }

        /// <summary>
        /// 只刷新当前显示element
        /// </summary>
        public virtual void RefreshCells()
        {
            foreach (var item in m_DisplayElementDict)
                m_OnElementChange.Invoke(item.Key, item.Value.go);
        }

        /// <summary>
        /// 刷新位置Element
        /// </summary>
        /// <param name="scrollPosition">scroll滑动的位置</param>
        /// <param name="normalize">scroll滑动的位置归一化</param>
        protected abstract void OnScroll(Vector2 scrollPosition, Vector2 normalize);

        protected virtual void OnElementChange(ScrollGridCell cell)
        {
            cell.go.transform.SetParent(scroll.viewport, false);
            cell.go.SetActive(true);
            cell.go.name = cell.index.ToString();
            m_OnElementChange.Invoke(cell.index, cell.go);
        }

        public abstract void ScrollTo(int index, float time);

        protected IEnumerator ScrollToCor(Vector2 scrollPosition, float time)
        {
            float now = 0;
            Vector2 oldPos = scroll.scrollPosition;
            while (now < time)
            {
                now += Time.deltaTime;
                m_OldScrollPosition = Vector2.Lerp(oldPos, scrollPosition, now / time);
                scroll.UpdateScrollPositionAndContentSize(m_OldScrollPosition, m_OldContentSize, true);
                yield return null;
            }
        }
    }
}