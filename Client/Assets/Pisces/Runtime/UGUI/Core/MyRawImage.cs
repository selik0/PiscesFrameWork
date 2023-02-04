/****************
 *@class name:		MyRawImage
 *@description:		继承于UnityEngine.UI.Image, 只做一些基础处理
 *@author:			selik0
 *@date:			2023-01-13 11:59:22
 *@version: 		V1.0.0
*************************************************************************/
namespace UnityEngine.UI
{
    public class MyRawImage : RawImage
    {
        new public Texture texture
        {
            get { return base.texture; }
            set { base.texture = value; }
        }
    }
}