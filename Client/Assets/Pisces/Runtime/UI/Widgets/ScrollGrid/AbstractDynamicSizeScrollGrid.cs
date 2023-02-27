/****************
 *@class name:		AbstractDynamicSizeScrollGrid
 *@description:		grid里的cell可以动态改变大小
 *@author:			selik0
 *@date:			2023-02-21 18:49:20
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
namespace UnityEngine.UI
{
    public class AbstractDynamicSizeScrollGrid : AbstractScrollGrid
    {
        public class ScrollGridFuncIntEvent : UnityFuncEvent<int, int> { }

        public ScrollGridFuncIntEvent onElementGoIndex = new ScrollGridFuncIntEvent();

        Dictionary<int, float> m_GroupContentSize = new Dictionary<int, float>();

        protected override int GetElementIndexByIndex(int index)
        {
            if (onElementGoIndex.IsEmpty())
                return 0;
            return onElementGoIndex.Invoke(index);
        }

        public void UpdateCellSize(int index, Vector2 size)
        {
            ScrollGridCell cell;
            if (!GetAndCreateCell(index, out cell))
                return;
            if (cell.nowSize != size)
            {
                int otherAxis = 1 - m_Axis;
                int remainder = cell.index % groupElementCount;
                int beginIndex = cell.index - remainder;
                int endIndex = Mathf.Min(beginIndex + groupElementCount, m_Count);
                Vector2 offsetSize = (size - cell.nowSize) / 2;

                float maxLen = 0;
                int groupIndex = cell.index / groupElementCount;
                UpdateContentSize(groupIndex, maxLen);

                cell.nowSize = size;

                ScrollGridCell groupCell;
                for (int i = beginIndex; i < endIndex; i++)
                {
                    if (GetCell(beginIndex, out groupCell))
                    {
                        if (i < cell.index)
                            groupCell.position[otherAxis] -= offsetSize[otherAxis];
                        else if (i > cell.index)
                            groupCell.position[otherAxis] += offsetSize[otherAxis];
                        groupCell.RefreshPosition(m_OldScrollPosition);
                    }
                }
                int idx = cell.index + groupElementCount;
                while (GetCell(idx, out groupCell))
                {
                    if (direction == Scroll.Direction.Vertical)
                        groupCell.position[m_Axis] -= offsetSize[m_Axis];
                    else
                        groupCell.position[m_Axis] += offsetSize[m_Axis];
                    groupCell.RefreshPosition(m_OldScrollPosition);
                    idx += groupElementCount;
                }
            }
        }

        protected override void UpdateCellPosition(ScrollGridCell cell)
        {
            ScrollGridCell lastCell;
            if (GetCell(cell.index - groupElementCount, out lastCell))
            {
                Vector2 len = lastCell.nowSize + (elementSizes[cell.goIndex] - elementSizes[lastCell.goIndex]) / 2;
                cell.position[m_Axis] = lastCell.position[m_Axis] - len[m_Axis];
            }
            else if (GetCell(cell.index + groupElementCount, out lastCell))
            {
                Vector2 len = cell.nowSize + (elementSizes[lastCell.goIndex] - elementSizes[cell.goIndex]) / 2;
                cell.position[m_Axis] = lastCell.position[m_Axis] + len[m_Axis];
            }
            else
            {
                float len = headPadding + m_OldScrollPosition[m_Axis] + elementSizes[0][m_Axis] / 2;
                if (direction == Scroll.Direction.Vertical)
                    cell.position[m_Axis] = scroll.viewport.rect.height / 2 - len;
                else
                    cell.position[m_Axis] = -scroll.viewport.rect.width / 2 + len;
            }

            // 一行的最后一个再刷新这一行的所有的坐标
            if ((cell.index + 1) % groupElementCount == 0 || cell.index + 1 == m_Count)
            {
                int otherAxis = 1 - m_Axis;
                int remainder = cell.index % groupElementCount;
                int beginIndex = cell.index - remainder;
                int endIndex = Mathf.Min(beginIndex + groupElementCount, m_Count);
                float[] lens = new float[groupElementCount];
                float maxLen = 0;
                for (int i = beginIndex; i < endIndex; i++)
                {
                    if (GetCell(i - beginIndex, out lastCell))
                    {
                        lens[i - beginIndex] = lastCell.nowSize[m_Axis] + elementSpacing[m_Axis];
                        maxLen = Mathf.Max(maxLen, lens[i - beginIndex]);
                    }
                }

                for (int i = beginIndex; i < endIndex; i++)
                {
                    if (GetCell(i, out lastCell))
                    {
                        for (int j = 0; j < groupElementCount; j++)
                        {
                            if (i == j) continue;
                            lastCell.position[otherAxis] += j < i ? lens[j] : -lens[j];
                        }
                        lastCell.position /= 2;
                        lastCell.RefreshPosition(m_OldScrollPosition);
                    }
                }

                UpdateContentSize(cell.index / groupElementCount, maxLen);
            }
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

            // 初始化位置
            UpdateCellPosition(cell);

            // 通过回调刷新cell的position, size
            OnElementChange(cell);

            if (!m_DisplayElementDict.ContainsKey(index))
                m_DisplayElementDict.Add(index, cell);
            bool isDisPlaying = cell.IsDisplaying(m_Axis, m_OldScrollPosition, scroll.viewport.rect.size[m_Axis]);
            return isDisPlaying;
        }

        protected override void DisplayCells(int beginIndex, bool isAdd)
        {
            // ScrollGridCell cellOne;
            // int preIndex = beginIndex - 1;
            // while (preIndex > 0 && !GetCell(preIndex, out cellOne))
            // {
            //     if (GetAndCreateCell(preIndex, out cellOne))
            //         UpdateCellPosition(cellOne);
            //     preIndex--;
            // }

            int key = 0;
            for (int i = 0; i < groupElementCount; i++)
            {
                int n = 0;
                bool isBeginShow = false, isShowComplete = false;
                while (!isBeginShow || !isShowComplete)
                {
                    key = beginIndex + i + groupElementCount * n;
                    n += isAdd ? 1 : -1;

                    if (DisplayCell(key))
                        isBeginShow = true;
                    else if (isBeginShow)
                        isShowComplete = true;
                }
            }
        }

        void UpdateContentSize(int index, float size)
        {
            if (m_GroupContentSize.ContainsKey(index))
            {
                float offset = size - m_GroupContentSize[index];
                m_OldContentSize[m_Axis] += size;
            }
            else
            {
                m_GroupContentSize.Add(index, size);
                m_OldContentSize[m_Axis] += size;
            }
            UpdateContentSize();
        }

        protected override void UpdateContentSize()
        {
            scroll.contentSize = m_OldContentSize;
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
            float delta = Mathf.Abs(scrollPosition[axis]) - Mathf.Abs(m_OldScrollPosition[axis]);

            // 记录数据 
            m_OldNormalize = normalize;
            m_OldScrollPosition = scrollPosition;

            int beginIndex = int.MaxValue;
            float viewLen = scroll.viewport.rect.size[axis];
            List<int> enqueueKeyList = new List<int>();
            foreach (var cell in m_DisplayElementDict.Values)
            {
                if (cell.IsDisplaying(axis, scrollPosition, viewLen))
                    beginIndex = Mathf.Min(beginIndex, cell.index);
                else
                {
                    ElementEnCacheQueue(cell);
                    enqueueKeyList.Add(cell.index);
                }
            }

            if (enqueueKeyList.Count == m_DisplayElementDict.Count)
                if (delta > 0)
                    beginIndex = Mathf.Max(enqueueKeyList.ToArray());
                else
                    beginIndex = Mathf.Min(enqueueKeyList.ToArray());

            if (enqueueKeyList.Count > 0)
            {
                foreach (var index in enqueueKeyList)
                    m_DisplayElementDict.Remove(index);
                enqueueKeyList.Clear();
            }
            DisplayCells(beginIndex, delta > 0);
        }

        public override void ScrollTo(int index, float time)
        {
            ScrollGridCell cell;
            ScrollGridCell firstCell;
            if (!GetCell(index, out cell) || !GetCell(0, out firstCell))
                return;

            Vector2 scrollPosition = cell.position - firstCell.position;
            if (time <= 0)
            {
                scroll.UpdateScrollPositionAndContentSize(scrollPosition, m_OldContentSize, true);
            }
            else
            {
                StartCoroutine(ScrollToCor(scrollPosition, time));
            }
        }
    }
}