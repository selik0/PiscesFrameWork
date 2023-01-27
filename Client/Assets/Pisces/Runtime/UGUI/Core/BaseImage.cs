/****************
 *@class name:		BaseImage
 *@description:		继承于UnityEngine.UI.Image, 只做一些基础处理
 *@author:			selik0
 *@date:			2023-01-09 15:02:49
 *@version: 		V1.0.0
*************************************************************************/
namespace UnityEngine.UI
{
    public class BaseImage : Image
    {
        new public Sprite sprite
        {
            get { return base.sprite; }
            set { base.sprite = value; }
        }
    }
}