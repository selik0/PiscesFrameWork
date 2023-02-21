using System.ComponentModel;
using System.Net.Mime;
/****************
 *@class name:		DynamicImage
 *@description:		专门用来加载动态图集里的图片
 *@author:			selik0
 *@date:			2023-02-07 19:02:28
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI
{
    public class DynamicImage : Image
    {
        private delegate void SpriteChanging(Sprite sprite);
        private SpriteChanging spriteChanging;
        public override Sprite sprite
        {
            get => base.sprite;
            set
            {
                spriteChanging?.Invoke(base.sprite);
                base.sprite = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            sprite = null;

            base.OnDestroy();
        }
    }
}