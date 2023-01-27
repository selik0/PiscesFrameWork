/****************
 *@class name:		IPanel
 *@description:		界面接口
 *@author:			selik0
 *@date:			2023-01-19 10:50:47
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pisces
{
    public interface IPanel
    {
        /// <summary>
        /// 打开界面的回调
        /// </summary>
        public void OnOpen();
        /// <summary>
        /// 关闭界面的回调
        /// </summary>
        public void OnClose();
        /// <summary>
        /// 隐藏界面的回调
        /// </summary>
        public void OnShow();
        /// <summary>
        /// 显示隐藏界面的回调
        /// </summary>
        public void OnHide();
    }
}