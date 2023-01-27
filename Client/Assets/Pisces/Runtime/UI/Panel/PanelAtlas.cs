/****************
 *@class name:		PanelAtlas
 *@description:		ui界面的图集管理，包括生成界面图集，界面引用的静态图集，界面引用的动态图集
 *@author:			selik0
 *@date:			2023-01-14 19:32:41
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
namespace Pisces
{
    public class PanelAtlas
    {
        public string selfAtlasName;
        public List<string> referenceStaticAtlasNameList;
        public List<string> referenceDynamicAtlasNameList;

        private List<SpriteAtlas> m_SpriteAtlasList;
        /// <summary>
        /// 还原界面图片
        /// </summary>
        public void RecoverSprite()
        {

        }

        public Sprite GetSprite(string name)
        {
            if (string.IsNullOrEmpty(name) || m_SpriteAtlasList == null || m_SpriteAtlasList.Count <= 0)
                return null;

            Sprite sprite;
            foreach (var atlas in m_SpriteAtlasList)
            {
              sprite = atlas.GetSprite(name);
              if (sprite != null)
                return sprite;
            }

            return null;
        }

#if UNITY_EDITOR
        public void Collect()
        {

        }
#endif
    }
}