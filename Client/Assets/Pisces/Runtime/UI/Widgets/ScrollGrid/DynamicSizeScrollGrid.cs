/****************
 *@class name:		DynamicSizeScrollGrid
 *@description:		grid里的cell可以动态改变大小
 *@author:			selik0
 *@date:			2023-02-21 18:49:20
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI
{
    public class DynamicSizeScrollGrid : AbstractScrollGrid
    {
        protected override ScrollGridCell CreateCell(int index)
        {
            throw new System.NotImplementedException();
        }

        protected override Vector2 GetCellSizeByIndex(int index)
        {
            throw new System.NotImplementedException();
        }

        protected override int GetElementIndexByIndex(int index)
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateContentSize()
        {
            throw new System.NotImplementedException();
        }
    }
}