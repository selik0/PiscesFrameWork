/****************
 *@class name:		DynamicSizeScrollGrid
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
    public class DynamicSizeScrollGrid : AbstractScrollGrid
    {
        public class ScrollGridFuncIntEvent : UnityFuncEvent<int, int> { }
        public class ScrollGridFuncVector2Event : UnityFuncEvent<int, Vector2> { }

        public ScrollGridFuncIntEvent onElementGoIndex = new ScrollGridFuncIntEvent();
        public ScrollGridFuncVector2Event onElementSize = new ScrollGridFuncVector2Event();
        protected override Vector2 GetElementSizeByIndex(int index)
        {
            if (onElementSize.IsEmpty())
                return new Vector2(100, 100);
            return onElementSize.Invoke(index);
        }

        protected override int GetElementIndexByIndex(int index)
        {
            return onElementGoIndex.Invoke(index);
        }
    }
}