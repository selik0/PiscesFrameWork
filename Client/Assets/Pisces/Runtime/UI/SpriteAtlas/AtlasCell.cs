/****************
 *@class name:		AtlasCell
 *@description:		动态图集的单个图片
 *@author:			selik0
 *@date:			2023-01-29 17:50:11
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pisces
{
    public class AtlasCell
    {
        public Rect rect;
        public int refCount;
        private Sprite asset;

        public AtlasCell(Rect rect_)
        {
            rect = rect_;
            refCount = 0;
        }

        public void SetSprite(Sprite sprite)
        {
            asset = sprite;
        }

        public Sprite GetSprite()
        {
            refCount++;
            return asset;
        }

        public bool IsNeedRelease()
        {
            return refCount <= 0;
        }
    }
}