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
    public abstract class AbstractScrollGrid : UIBehaviour
    {
        [Serializable]
        /// <summary>
        /// scrollGrid元素改变时的回调
        /// </summary>
        public class ScrollGridEvent : UnityEvent<GameObject, int> { }

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

        public int headPadding;
        public int tailPadding;

        [Tooltip("一行或一列中element的数量")]
        [Range(1, 10)]
        public int groupElementCount = 1;

        [Tooltip("Element Prefab的大小")]
        public List<Vector2> elementSizes = new List<Vector2>();

        [Tooltip("Element Prefab之间的间隔")]
        public Vector2 elementSpacing = Vector2.zero;
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

        protected int m_Axis => direction == Scroll.Direction.Vertical ? 1 : 0;
        // 当前的最小element的下标
        protected int m_CurrentDisplayElementIndex = 0;

        protected Vector2 m_OldNormalizePosition;
        protected Vector2 m_OldScrollPosition;

        protected Dictionary<int, Queue<GameObject>> m_CacheElementDict = new Dictionary<int, Queue<GameObject>>();

        protected Dictionary<int, ScrollGridCell> m_DisplayElementDict = new Dictionary<int, ScrollGridCell>();

        protected Dictionary<int, ScrollGridCell> m_TotalElementDict = new Dictionary<int, ScrollGridCell>();

        private Vector2 m_OldViewSize;
        protected override void Awake()
        {
            foreach (var go in m_ElementPrefabs)
            {
                go?.SetActive(false);
            }
            scroll.onValueChanged.AddListener(UpdatePosition);
        }

        protected override void OnDestroy()
        {
            scroll.onValueChanged.RemoveListener(UpdatePosition);
        }

        /// <summary>
        /// 根据下标获取对应的Cell数据,未获取到则创建
        /// </summary>
        /// <param name="index">下标</param>
        /// <param name="cell">cell数据类</param>
        /// <returns>是否获取到</returns>
        protected bool GetAndCreateCell(int index, out ScrollGridCell cell)
        {
            cell = null;
            if (index < 0 || index > m_Count)
                return false;

            if (m_DisplayElementDict.TryGetValue(index, out cell))
                return true;

            if (m_TotalElementDict.TryGetValue(index, out cell))
                return true;

            cell = CreateCell(index);
            return true;
        }

        protected bool GetCell(int index, out ScrollGridCell cell)
        {
            cell = null;
            if (index < 0 || index > m_Count)
                return false;

            if (m_DisplayElementDict.TryGetValue(index, out cell))
                return true;

            if (m_TotalElementDict.TryGetValue(index, out cell))
                return true;

            return false;
        }

        /// <summary>
        /// 根据下标创建Cell数据类
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>Cell数据类</returns>
        protected abstract ScrollGridCell CreateCell(int index);
        /// <summary>
        /// 根据下标获取Cell的大小elementSize + elementSpacing
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>CellSize</returns>
        protected abstract Vector2 GetCellSizeByIndex(int index);
        /// <summary>
        /// 根据下标选择elementPrefab
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>elementPrefabs的下标</returns>
        protected abstract int GetElementIndexByIndex(int index);

        /// <summary>
        /// 根据下标获取实例化的Element
        /// </summary>
        /// <param name="index">grid cell的下标</param>
        /// <returns>实例化的element</returns>
        protected GameObject CreateElement(int index)
        {
            GameObject go = null;
            Queue<GameObject> queue = null;

            int goIndex = GetElementIndexByIndex(index);

            // 从缓存里拿
            if (m_CacheElementDict.TryGetValue(goIndex, out queue))
                go = queue.Dequeue();

            // 创建一个新的Go
            if (null == go && m_ElementPrefabs.Count > goIndex && null != m_ElementPrefabs[goIndex])
                go = Instantiate<GameObject>(m_ElementPrefabs[goIndex]);

            return go;
        }


        protected virtual void RecycleCell(int idx, ScrollGridCell cell)
        {
            Queue<GameObject> queue = null;
            if (!m_CacheElementDict.TryGetValue(cell.goIndex, out queue))
            {
                queue = new Queue<GameObject>();
                m_CacheElementDict.Add(cell.goIndex, queue);
            }
            // cell.go.SetActive(false);
            queue.Enqueue(cell.go);
            cell.go = null;
        }

        /// <summary>
        /// 初始化所有的cell数据
        /// </summary>
        /// <param name="count">cell的数量</param>
        /// <param name="isReset">是否重新初始化数据</param>
        protected virtual void InitializeTotalCell(int count, bool isReset)
        {
            if (isReset) m_TotalElementDict.Clear();

            if (m_TotalElementDict.Count > count)
            {
                int endIndex = m_TotalElementDict.Count;
                for (int i = count; i < endIndex; i++)
                    m_TotalElementDict.Remove(i);
            }
            else if (m_TotalElementDict.Count < count)
            {
                ScrollGridCell cell = null;
                int beginIndex = m_TotalElementDict.Count;
                for (int i = beginIndex; i < count; i++)
                {
                    if (GetAndCreateCell(i, out cell))
                        m_TotalElementDict.Add(i, cell);
                }
            }
        }

        protected abstract void UpdateContentSize();

        /// <summary>
        /// 刷新grid中element的数量
        /// </summary>
        /// <param name="count">最新的数量</param>
        /// <param name="isReset">是否重置cell数据</param>
        public virtual void UpdateCount(int count, bool isReset, bool isScrollToHead)
        {
            m_Count = count;

            scroll.StopMovement();
            // 初始化cell
            InitializeTotalCell(count, isReset);
            // 更新滑动的长度
            UpdateContentSize();

            if (isReset)
            {
                // 先回收所有显示
                if (m_DisplayElementDict.Count > 0)
                    foreach (var item in m_DisplayElementDict)
                        RecycleCell(item.Key, item.Value);
                m_DisplayElementDict.Clear();
                ScrollGridCell cell;
                int axis = m_Axis;
                float viewlen = scroll.viewport.rect.size[axis];
                for (int i = 0; i < groupElementCount; i++)
                {
                    bool isLoop = true;
                    int n = 0;
                    while (isLoop)
                    {
                        int key = m_CurrentDisplayElementIndex + i + groupElementCount * n;
                        if (GetCell(key, out cell) && cell.IsDisplaying(axis, m_OldScrollPosition, viewlen))
                        {
                            OnCellElementChagne(key, cell);
                            cell.RefreshPosition(m_OldScrollPosition);
                            n++;
                        }
                        else
                            isLoop = false;
                    }
                }
            }

            // 重新显示
            if (isScrollToHead)
                scroll.scrollPosition = Vector2.zero;
            else
            {
                foreach (var cell in m_DisplayElementDict.Values)
                    cell.RefreshPosition(m_OldScrollPosition);
            }
        }

        /// <summary>
        /// 只刷新当前显示element
        /// </summary>
        public virtual void RefreshElements()
        {
            foreach (var item in m_DisplayElementDict)
                m_OnElementChange.Invoke(item.Value.go, item.Key);
        }

        protected virtual void OnCellElementChagne(int index, ScrollGridCell cell)
        {
            cell.SetTransform(CreateElement(index));
            m_DisplayElementDict.Add(index, cell);
            cell.go.transform.SetParent(scroll.viewport, false);
            cell.go.SetActive(true);
            onElementChange.Invoke(cell.go, index);
        }

        /// <summary>
        /// 刷新位置Element
        /// </summary>
        /// <param name="scrollPosition">scroll滑动的位置</param>
        /// <param name="normalize">scroll滑动的位置归一化</param>
        protected virtual void UpdatePosition(Vector2 scrollPosition, Vector2 normalize)
        {
            int axis = m_Axis;
            int currentIndex = int.MaxValue;
            Debug.Log($"sssss {scrollPosition}  {normalize}");
            if (normalize[axis] < 0 || normalize[axis] > 1)
            {
                foreach (var item in m_DisplayElementDict)
                {
                    item.Value.RefreshPosition(scrollPosition);
                    currentIndex = Mathf.Min(currentIndex, item.Key);
                }
            }
            else
            {
                bool isVertical = direction == Scroll.Direction.Vertical;
                float delta = normalize[axis] - m_OldNormalizePosition[axis];
                // 确定滑动方向
                bool isScrollToTail = delta > 0;

                float viewlen = scroll.viewport.rect.size[axis];
                // 获取需要回收的key
                List<int> keyList = new List<int>();
                Dictionary<int, int> displayBeginDict = new Dictionary<int, int>();
                if (isScrollToTail)
                {
                    foreach (var item in m_DisplayElementDict)
                    {
                        if (!item.Value.IsDisplaying(axis, scrollPosition, viewlen))
                            keyList.Add(item.Key);
                        else
                        {
                            item.Value.RefreshPosition(scrollPosition);

                            int remainder = item.Key % groupElementCount;
                            if (displayBeginDict.ContainsKey(remainder))
                                displayBeginDict[remainder] = Mathf.Max(displayBeginDict[remainder], item.Key);
                            else
                                displayBeginDict.Add(remainder, item.Key);

                            currentIndex = Mathf.Min(currentIndex, item.Key);
                        }
                    }
                }
                else
                {
                    foreach (var item in m_DisplayElementDict)
                    {
                        if (!item.Value.IsDisplaying(axis, scrollPosition, viewlen))
                            keyList.Add(item.Key);
                        else
                        {
                            item.Value.RefreshPosition(scrollPosition);

                            int remainder = item.Key % groupElementCount;
                            if (displayBeginDict.ContainsKey(remainder))
                                displayBeginDict[remainder] = Mathf.Min(displayBeginDict[remainder], item.Key);
                            else
                                displayBeginDict.Add(remainder, item.Key);

                            currentIndex = Mathf.Min(currentIndex, item.Key);
                        }
                    }
                }
                if (keyList.Count > 0)
                {
                    foreach (var key in keyList)
                    {
                        RecycleCell(key, m_DisplayElementDict[key]);
                        m_DisplayElementDict.Remove(key);
                    }
                    keyList.Clear();

                    ScrollGridCell cell;
                    foreach (var key in displayBeginDict.Values)
                    {
                        bool isLoop = true;
                        int n = 1;
                        while (isLoop)
                        {
                            int index = key;
                            if (isScrollToTail)
                                index += groupElementCount * n;
                            else
                                index -= groupElementCount * n;
                            if (GetAndCreateCell(index, out cell) && cell.IsDisplaying(axis, scrollPosition, viewlen))
                            {
                                OnCellElementChagne(index, cell);
                                cell.RefreshPosition(scrollPosition);
                                currentIndex = Mathf.Min(currentIndex, index);
                                n++;
                            }
                            else
                            {
                                isLoop = false;
                            }
                        }
                    }
                }
            }
            m_CurrentDisplayElementIndex = currentIndex;
            m_OldNormalizePosition = normalize;
        }

        public virtual void ScrollTo(int index, float time)
        {
            scroll.StopMovement();

            ScrollGridCell firstCell = null;
            ScrollGridCell targetCell = null;
            var pos = Vector2.zero;
            if (GetCell(index, out targetCell) && GetCell(0, out firstCell))
            {
                pos[m_Axis] = targetCell.position[m_Axis] - firstCell.position[m_Axis];
            }
            if (time <= 0)
                scroll.scrollPosition = pos;
            else
                StartCoroutine(ScrollToCor(time, pos));
        }

        IEnumerator ScrollToCor(float time, Vector2 targetPos)
        {
            float now = 0;
            Vector2 originPos = scroll.scrollPosition;
            while (now < time)
            {
                now += Time.deltaTime;
                scroll.scrollPosition = Vector2.Lerp(originPos, targetPos, now / time);
                yield return null;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            Debug.Assert(scroll.viewport != null);

            if (Application.isPlaying && m_OldViewSize != scroll.viewport.rect.size)
            {
                Debug.Log("Scroll Gride OnRectTransformDimensionsChange" + scroll.viewport.rect.size);
                InitializeTotalCell(m_Count, true);
                UpdateContentSize();
                m_OldViewSize = scroll.viewport.rect.size;
            }
        }
    }
}