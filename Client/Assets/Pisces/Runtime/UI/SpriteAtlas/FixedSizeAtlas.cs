using System;
/****************
 *@class name:		FixedSizeAtlas
 *@description:		固定大小的动态图集
 *@author:			selik0
 *@date:			2023-01-29 17:34:58
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
namespace Pisces
{
    public class FixedSizeAtlas
    {
        private string m_AtlasName = "";
        private Vector2 m_AtlasSize = new Vector2(1024, 1024);
        private Vector2 m_CellSize = new Vector2(100, 100);
        private Texture2D m_Atlas;
        private TextureFormat m_TextureFormat = TextureFormat.ASTC_4x4;

        private Dictionary<string, AtlasCell> m_SpriteDict = new Dictionary<string, AtlasCell>();
        private Queue<AtlasCell> m_CacheQueue = new Queue<AtlasCell>(10);

        public FixedSizeAtlas(string name, Vector2 atlasSize, Vector2 cellSize, TextureFormat format)
        {
            m_AtlasName = name;
            m_AtlasSize = atlasSize;
            m_CellSize = cellSize;
            m_TextureFormat = format;
            m_Atlas = new Texture2D(Mathf.CeilToInt(atlasSize.x), Mathf.CeilToInt(atlasSize.y), format, false);
            InitializeCacheQueue();
        }

        void InitializeCacheQueue()
        {
            if (m_Atlas == null) return;
            int col = Mathf.FloorToInt(m_AtlasSize.x / m_CellSize.x);
            int row = Mathf.FloorToInt(m_AtlasSize.y / m_CellSize.y);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    int x = Mathf.FloorToInt(j * m_CellSize.x);
                    int y = Mathf.FloorToInt(i * m_CellSize.y);
                    Rect rect = new Rect(x, y, m_CellSize.x, m_CellSize.y);
                    AtlasCell cell = new AtlasCell(rect);
                    cell.SetSprite(Sprite.Create(m_Atlas, rect, Vector2.one * .5f, 100, 0, SpriteMeshType.FullRect));
                    m_CacheQueue.Enqueue(cell);
                }
            }
        }

        public Sprite GetSprite(string key)
        {
            return null;
        }

        public void RecycleSprite(string key)
        {

        }

        /// <summary>
        /// 加载key对应的图片并合并到图集上
        /// </summary>
        /// <param name="key">图片的路径</param>
        void AddSprite(string key)
        {

        }

        /// <summary>
        /// 合并图片到图集
        /// </summary>
        /// <param name="spriteTex">图片的texture2d</param>
        void CombineBlock(Texture2D spriteTex, AtlasCell cell)
        {
            int blockWidth = 4;
            int blockHeight = 4;
            int blockByte = 16;
            bool isSupport = true;
            switch (m_TextureFormat)
            {
                case TextureFormat.RGBA32:
                    blockWidth = 16;
                    blockHeight = 8;
                    blockByte = 512;
                    break;
                case TextureFormat.ASTC_4x4:
                    break;
                case TextureFormat.ASTC_5x5:
                    blockWidth = 5;
                    blockHeight = 5;
                    break;
                case TextureFormat.ASTC_6x6:
                    blockWidth = 6;
                    blockHeight = 6;
                    break;
                default:
                    isSupport = false;
                    break;
            }
            if (!isSupport) return;
            byte[] src = spriteTex.GetRawTextureData();
            byte[] dest = m_Atlas.GetRawTextureData();
            // 图片的宽高的像素块的数量
            int spriteWidthBlockNum = Mathf.CeilToInt(spriteTex.width / blockWidth);
            int spriteHeightBlockNum = Mathf.CeilToInt(spriteTex.height / blockHeight);
            // 图集的宽的像素块的数量
            int atlasWidthBlockNum = Mathf.CeilToInt(m_Atlas.width / blockWidth);

            int copyLen = src.Length / spriteWidthBlockNum;
            int atlasLen = dest.Length / atlasWidthBlockNum;

            int srcIndex = 0, destIndex = 0;
            int destx = Mathf.CeilToInt(cell.rect.x / blockWidth);
            int desty = Mathf.CeilToInt(cell.rect.y / blockHeight);
            for (int i = 0; i < spriteHeightBlockNum; i++)
            {
                srcIndex = copyLen * i;
                destIndex = destx * blockByte + (desty + i) * atlasLen;
                Buffer.BlockCopy(src, srcIndex, dest, destIndex, copyLen);
            }
            m_Atlas.LoadRawTextureData(dest);
            m_Atlas.Apply();
        }

        // struct CombineJob :IJob
        // {
        //     public int blockWidth = 4;
        // }
    }
}