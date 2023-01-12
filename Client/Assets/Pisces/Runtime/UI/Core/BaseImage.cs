using System.Net.Mime;
/****************
 *@class name:		BaseImage
 *@description:		
 *@author:			selik0
 *@date:			2023-01-09 15:02:49
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI
{
    public class BaseImage : Image
    {
        public new Sprite sprite
        {
            get { return base.sprite; }
            set { base.sprite = value; }
        }
    }
}