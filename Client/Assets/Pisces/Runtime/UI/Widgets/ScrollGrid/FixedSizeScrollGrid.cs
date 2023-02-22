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
        protected override Vector2 GetElementSizeByIndex(int index)
        {
            if (elementSizes.Count > 0)
                return elementSizes[0];
            return new Vector2(100, 100);
        }

        protected override int GetElementIndexByIndex(int index)
        {
            return 0;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            Debug.Assert(scroll.viewport != null);

            if (Application.isPlaying && m_OldViewSize != scroll.viewport.rect.size)
            {
                if (isAutoGroupElementCount)
                {
                    int otherAxis = 1 - m_Axis;
                    groupElementCount = Mathf.Max(minGroupElementCount, Mathf.FloorToInt(scroll.viewport.rect.size[otherAxis] / (GetElementSizeByIndex(0) + elementSpacing)[otherAxis]));
                    UpdateCount(m_Count, true, false);
                }
                m_OldViewSize = scroll.viewport.rect.size;
            }
        }
    }
}