/****************
 *@class name:		FixedSizeScrollGrid
 *@description:	    自定义的固定item大小的布局类，可以单独使用也可以和scroll组合变成滑动布局
 *@author:			selik0
 *@date:			2023-02-07 19:24:58
 *@version: 		V1.0.0
*************************************************************************/
namespace UnityEngine.UI
{
    public class FixedSizeScrollGrid : AbstractScrollGrid
    {
        // [Tooltip("生成的element没有超过视图显示的最大数量时居中显示")]
        // public bool isInCenterWithNonSilding;

        [Tooltip("是否根据宽度自动适配一行或一列的Element数量")]
        public bool isAutoGroupElementCount = false;

        [Tooltip("自适应行列数量时的最小数量")]
        [Range(1, 10)]
        public int minGroupElementCount = 1;

        protected Vector2 m_OldViewSize;

        private Vector2 beginPosition = Vector2.zero;
        protected override int GetElementIndexByIndex(int index)
        {
            return 0;
        }

        protected override void OnElementChange(ScrollGridCell cell)
        {
            base.OnElementChange(cell);
            UpdateCellSize(cell.index, elementSizes[0]);
        }

        protected override void UpdateCellPosition(ScrollGridCell cell)
        {
            int otherAxis = 1 - m_Axis;
            int col = cell.index % groupElementCount;
            int row = cell.index / groupElementCount;
            Vector2 cellSize = cell.nowSize + elementSpacing;

            bool isVertical = direction == Scroll.Direction.Vertical;

            cell.position[m_Axis] = beginPosition[m_Axis] + (isVertical ? -cellSize[m_Axis] : cellSize[m_Axis]) * row;
            cell.position[otherAxis] = beginPosition[otherAxis] + cellSize[otherAxis] * col;
        }

        protected override void UpdateContentSize()
        {
            int row = Mathf.CeilToInt(m_Count / (float)groupElementCount);
            m_OldContentSize[m_Axis] = headPadding + tailPadding - elementSpacing[m_Axis] + (elementSizes[0][m_Axis] + elementSpacing[m_Axis]) * row;
            scroll.contentSize = m_OldContentSize;
        }

        public override void ScrollTo(int index, float time)
        {
            Vector2 scrollPosition = Vector2.zero;
            int row = index / groupElementCount;
            Vector2 cellSize = elementSizes[0] + elementSpacing;
            scrollPosition[m_Axis] = cellSize[m_Axis] * row;

            if (time <= 0)
            {
                m_OldScrollPosition = scrollPosition;
                foreach (var cell in m_DisplayElementDict.Values)
                    ElementEnCacheQueue(cell);
                m_DisplayElementDict.Clear();

                int beginIndex = index - index % groupElementCount;
                DisplayCells(beginIndex, true);
                scroll.UpdateScrollPositionAndContentSize(m_OldScrollPosition, m_OldContentSize);
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
                beginPosition.x = -(groupElementCount - 1) / 2f * (elementSizes[0][otherAxis] + elementSpacing[otherAxis]);
                beginPosition.y = scroll.viewport.rect.height / 2 - headPadding - elementSizes[0][m_Axis] / 2;
            }
            else
            {
                beginPosition.x = -scroll.viewport.rect.width + headPadding + elementSizes[0][m_Axis] / 2;
                beginPosition.y = -(groupElementCount - 1) / 2f * (elementSizes[0][otherAxis] + elementSpacing[otherAxis]);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            Debug.Assert(scroll.viewport != null);

            if (Application.isPlaying && m_OldViewSize != scroll.viewport.rect.size)
            {
                InitializeBeginPosition();
                if (isAutoGroupElementCount)
                {
                    int otherAxis = 1 - m_Axis;
                    groupElementCount = Mathf.Max(minGroupElementCount, Mathf.FloorToInt(scroll.viewport.rect.size[otherAxis] / (elementSizes[0] + elementSpacing)[otherAxis]));
                    UpdateCount(m_Count, true, false);
                }
                m_OldViewSize = scroll.viewport.rect.size;
            }
        }
    }
}