/****************
 *@class name:		AbstractFixedSizeScrollGrid
 *@description:	    自定义的固定item大小的布局类，可以单独使用也可以和scroll组合变成滑动布局
 *@author:			selik0
 *@date:			2023-02-07 19:24:58
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
namespace UnityEngine.UI
{
    public abstract class AbstractFixedSizeScrollGrid : AbstractScrollGrid
    {
        // [Tooltip("生成的element没有超过视图显示的最大数量时居中显示")]
        // public bool isInCenterWithNonSilding;

        [Tooltip("是否根据宽度自动适配一行或一列的Element数量")]
        public bool isAutoGroupElementCount = false;

        [Tooltip("自适应行列数量时的最小数量")]
        [Range(1, 10)]
        public int minGroupElementCount = 1;

        protected Vector2 m_OldViewSize;
        private int m_GroupIndex = 0;
        private int m_GroupCount = 0;
        private Vector2 m_BeginPosition = Vector2.zero;
        protected override int GetElementIndexByIndex(int index)
        {
            return 0;
        }

        protected override void UpdateCellPosition(ScrollGridCell cell)
        {
            int otherAxis = 1 - m_Axis;
            int col = cell.index % groupElementCount;
            int row = cell.index / groupElementCount;
            Vector2 cellSize = cell.nowSize + elementSpacing;

            bool isVertical = direction == Scroll.Direction.Vertical;

            cell.position[m_Axis] = m_BeginPosition[m_Axis] + (isVertical ? -cellSize[m_Axis] : cellSize[m_Axis]) * row;
            cell.position[otherAxis] = m_BeginPosition[otherAxis] + cellSize[otherAxis] * col;
            // 设置cell.go的localPosition
            cell.RefreshPosition(m_OldScrollPosition);
        }

        protected override bool DisplayCell(int index)
        {
            ScrollGridCell cell;
            if (!GetAndCreateCell(index, out cell))
                return false;

            if (null != cell.go)
            {
                if (!m_DisplayElementDict.ContainsKey(cell.index))
                    m_DisplayElementDict.Add(cell.index, cell);
                cell.RefreshPosition(m_OldScrollPosition);
                return true;
            }

            // 没有拿到ElementPrefab
            if (!ElementDeCacheQueue(cell))
                return false;

            if (!m_DisplayElementDict.ContainsKey(cell.index))
                m_DisplayElementDict.Add(cell.index, cell);
            // 初始化位置
            UpdateCellPosition(cell);

            OnElementChange(cell);

            UpdateContentSize();

            return true;
        }

        protected override void DisplayCells(int beginIndex, bool isAdd)
        {
            int endIndex = beginIndex + m_GroupCount * groupElementCount;
            for (int i = beginIndex; i < endIndex; i++)
                DisplayCell(i);
        }

        protected override void OnScroll(Vector2 scrollPosition, Vector2 normalize)
        {
            int axis = m_Axis;
            if (normalize[axis] < 0 || normalize[axis] > 1)
            {
                foreach (var cell in m_DisplayElementDict.Values)
                    cell.RefreshPosition(scrollPosition);
                return;
            }
            float delta = normalize[axis] - m_OldNormalize[axis];
            // 记录数据 
            m_OldNormalize = normalize;
            m_OldScrollPosition = scrollPosition;

            ScrollGridCell cellOne;
            m_GroupIndex = Mathf.FloorToInt((scrollPosition[axis] + elementSpacing[axis]) / (elementSizes[0][axis] + elementSpacing[axis]));
            int beginIndex = m_GroupIndex * groupElementCount;
            if (m_DisplayElementDict.ContainsKey(beginIndex - 1))
            {
                Debug.Log(1);
                // 回收
                // 表示从头滑到尾
                if (delta > 0)
                {
                    int n = 1;
                    while (m_DisplayElementDict.TryGetValue(beginIndex - n, out cellOne))
                    {
                        ElementEnCacheQueue(cellOne);
                        m_DisplayElementDict.Remove(beginIndex - n);
                        n++;
                    }
                }
                else if (delta < 0)
                {
                    int n = m_GroupCount * groupElementCount;
                    while (m_DisplayElementDict.TryGetValue(beginIndex + n, out cellOne))
                    {
                        ElementEnCacheQueue(cellOne);
                        m_DisplayElementDict.Remove(beginIndex + n);
                        n++;
                    }
                }
            }
            else
            {
                Debug.Log(2);
                foreach (var cell in m_DisplayElementDict.Values)
                    ElementEnCacheQueue(cell);
                m_DisplayElementDict.Clear();
            }
            // 显示
            DisplayCells(beginIndex, true);
        }

        protected override void UpdateContentSize()
        {
            int row = Mathf.CeilToInt(m_Count / (float)groupElementCount);
            m_OldContentSize[m_Axis] = headPadding + tailPadding - elementSpacing[m_Axis] + (elementSizes[0][m_Axis] + elementSpacing[m_Axis]) * row;
            scroll.contentSize = m_OldContentSize;
        }

        public int GetCurrentGroupIndex()
        {
            return m_GroupIndex;
        }

        public override void ScrollTo(int index, float time)
        {
            Vector2 scrollPosition = Vector2.zero;
            Vector2 cellSize = elementSizes[0] + elementSpacing;
            m_GroupIndex = index / groupElementCount;
            scrollPosition[m_Axis] = cellSize[m_Axis] * m_GroupIndex + headPadding - elementSpacing[m_Axis];
            if (direction == Scroll.Direction.Horizontal)
                scrollPosition *= -1;

            if (time <= 0)
            {
                scroll.UpdateScrollPositionAndContentSize(scrollPosition, m_OldContentSize, true);
            }
            else
            {
                StartCoroutine(ScrollToCor(scrollPosition, time));
            }
        }

        void InitializeBeginPosition()
        {
            int otherAxis = 1 - m_Axis;
            if (direction == Scroll.Direction.Vertical)
            {
                m_BeginPosition.x = -(groupElementCount - 1) / 2f * (elementSizes[0][otherAxis] + elementSpacing[otherAxis]);
                m_BeginPosition.y = scroll.viewport.rect.height / 2 - headPadding - elementSizes[0][m_Axis] / 2;
            }
            else
            {
                m_BeginPosition.x = -scroll.viewport.rect.width + headPadding + elementSizes[0][m_Axis] / 2;
                m_BeginPosition.y = -(groupElementCount - 1) / 2f * (elementSizes[0][otherAxis] + elementSpacing[otherAxis]);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            Debug.Assert(scroll.viewport != null);

            if (Application.isPlaying && m_OldViewSize != scroll.viewport.rect.size)
            {
                if (isAutoGroupElementCount)
                {
                    int otherAxis = 1 - m_Axis;
                    Debug.Log(scroll.viewport.rect.size[otherAxis]);
                    groupElementCount = Mathf.Max(minGroupElementCount, Mathf.FloorToInt(scroll.viewport.rect.size[otherAxis] / (elementSizes[0] + elementSpacing)[otherAxis]));
                    InitializeBeginPosition();
                    UpdateCount(m_Count, true, false);
                }
                else
                    InitializeBeginPosition();
                m_OldViewSize = scroll.viewport.rect.size;
                m_GroupCount = Mathf.CeilToInt((m_OldViewSize[m_Axis] + elementSpacing[m_Axis]) / (elementSizes[0][m_Axis] + elementSpacing[m_Axis]));
            }
        }
    }
}