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

        protected override ScrollGridCell CreateCell(int index)
        {
            ScrollGridCell cell;
            Vector2 position = Vector2.zero;
            Vector2 size = GetCellSizeByIndex(index);
            float viewHalfLen = scroll.viewport.rect.size[m_Axis] / 2;

            if (direction == Scroll.Direction.Vertical)
            {
                position.x = ((1 - groupElementCount) / 2f + (index % groupElementCount)) * (size.x + elementSpacing.x);
                position.y = viewHalfLen - headPadding - size.y / 2 - (index / groupElementCount) * (size.y + elementSpacing.y);
            }
            else
            {
                position.x = -viewHalfLen + headPadding + size.x / 2 - (index / groupElementCount) * (size.x + elementSpacing.x);
                position.y = ((1 - groupElementCount) / 2f + (index % groupElementCount)) * (size.y + elementSpacing.y);
            }
            cell = new ScrollGridCell(position, size);
            return cell;
        }

        protected override Vector2 GetCellSizeByIndex(int index)
        {
            if (elementSizes.Count > 0)
                return elementSizes[0];
            return new Vector2(100, 100);
        }

        protected override int GetElementIndexByIndex(int index)
        {
            return 0;
        }

        protected override void UpdateContentSize()
        {
            Debug.Assert(elementSizes.Count > 0);
            Vector2 contentSize = Vector2.zero;
            Vector2 elementSize = elementSizes[0];
            contentSize[m_Axis] = headPadding + tailPadding + (elementSize[m_Axis] + elementSpacing[m_Axis]) * Mathf.CeilToInt(m_Count / (float)groupElementCount);
            scroll.contentSize = contentSize;
        }
    }
}