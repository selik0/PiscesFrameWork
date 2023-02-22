/****************
 *@class name:		GridCell
 *@description:		grid里的cell数据类
 *@author:			selik0
 *@date:			2023-02-07 21:18:37
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
namespace UnityEngine.UI
{
    public class ScrollGridCell
    {
        /// <summary>
        /// 当前Cell使用的Prefab的下标
        /// </summary>
        public int goIndex;
        public Vector2 position = Vector2.zero;
        public GameObject go;
        private RectTransform transform;
        public ScrollGridCell() { }
        public ScrollGridCell(Vector2 position_)
        {
            position = position_;
        }

        public void RefreshPosition(Vector2 scrollPosition)
        {
            if (null == go) return;
            transform.localPosition = position + scrollPosition;
        }

        public void SetTransform(GameObject go_)
        {
            if (null == go_) return;
            go = go_;
            transform = go.transform as RectTransform;
        }

        public void RecycleTransform(Queue<GameObject> queue)
        {
            if (go == null) return;
            queue.Enqueue(go);
            go = null;
            transform = null;
        }

        public bool IsDisplaying(int axis, Vector2 scrollPosition, float len)
        {
            float nowPos = position[axis] + scrollPosition[axis];
            if (nowPos <= len && nowPos >= -len)
                return true;
            return false;
        }
    }
}