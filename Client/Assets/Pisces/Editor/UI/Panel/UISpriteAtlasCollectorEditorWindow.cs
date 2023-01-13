using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using Pisces;
namespace PiscesEditor
{
    public class UISpriteAtlasCollectorEditorWindow : EditorWindow
    {
        static UISpriteAtlasCollectorEditorWindow window;
        [MenuItem("FrameWork/UIControlWindow")]
        static void CreatWindow()
        {
            window = GetWindow<UISpriteAtlasCollectorEditorWindow>();
            window.StartCoroutine("CreateAtlas");
        }
        // private int mCurrentToolbarIndex;
        private bool bIsCollectPath = false;
        private bool bIsSetAtlas = false;
        private int progress = 0;
        private int maxProgress = 0;
        private Dictionary<string, List<string>> atlasSpritePathDic = new Dictionary<string, List<string>>();
        private void OnGUI()
        {
            // mCurrentToolbarIndex=GUILayout.Toolbar(mCurrentToolbarIndex, new[] { "1", "2", "3", "4", "5" });
            if (bIsCollectPath)
            {
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("收集界面信息", "进度：" + progress + "/" + maxProgress, progress / (float)maxProgress);
                if (isCancel)
                {
                    this.StopAllCoroutines();
                }
            }
            if (bIsSetAtlas)
            {
                EditorUtility.DisplayProgressBar("创建设置图集", "进度：" + progress + "/" + maxProgress, progress / (float)maxProgress);
            }

        }

        IEnumerator CreateAtlas()
        {
            Debug.Log("开始配置UI图集");
            // 先收集图集信息
            // 先检查文件夹是否存在
            if (!Directory.Exists(EditorPathUtility.UIPrefabsSavePath))
            {
                Directory.CreateDirectory(EditorPathUtility.UIPrefabsSavePath);
                this.StopAllCoroutines();
                yield return null;
            }
            // 收集所以界面的图集的图片路径
            bIsCollectPath = true;
            UISpriteAtlasCollector[] collectors;
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new string[] { EditorPathUtility.UIPrefabsSavePath });
            progress = 0;
            maxProgress = prefabGuids.Length;
            Debug.Log("收集的prefab数量" + prefabGuids.Length);
            foreach (string guid in prefabGuids)
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                collectors = go.GetComponentsInChildren<UISpriteAtlasCollector>(true);
                if (null != collectors && collectors.Length > 0)
                {
                    foreach (var collector in collectors)
                    {
                        if (string.IsNullOrEmpty(collector.atlasName))
                            continue;
                        List<string> spriteList;
                        if (!atlasSpritePathDic.TryGetValue(collector.atlasName, out spriteList))
                        {
                            spriteList = new List<string>();
                            atlasSpritePathDic.Add(collector.atlasName, spriteList);
                        }
                        foreach (var path in collector.spritePaths)
                            if (!spriteList.Contains(path))
                                spriteList.Add(path);
                    }
                }
                progress++;
                yield return null;
            }
            bIsCollectPath = false;
            EditorUtility.ClearProgressBar();
            Debug.Log("图集信息收集完成");
            // 先检查文件夹是否存在
            if (!Directory.Exists(EditorPathUtility.SpriteAtlasExportDirectory))
            {
                Directory.CreateDirectory(EditorPathUtility.SpriteAtlasExportDirectory);
            }
            // 获取已经创建的图集，并清空数据，等待重新赋值
            Dictionary<string, SpriteAtlas> spriteAtlasDic = new Dictionary<string, SpriteAtlas>();
            string[] spriteAtlasGuids = AssetDatabase.FindAssets("t:SpriteAtlas", new string[] { EditorPathUtility.SpriteAtlasExportDirectory });
            foreach (string guid in spriteAtlasGuids)
            {
                SpriteAtlas item = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AssetDatabase.GUIDToAssetPath(guid));
                UnityEngine.Object[] objects = item.GetPackables();
                item.Remove(objects);
                spriteAtlasDic.Add(item.name, item);
            }
            yield return null;
            // 根据收集到的图集信息重新赋值图集
            bIsSetAtlas = true;
            progress = 0;
            maxProgress = atlasSpritePathDic.Count;
            Debug.Log(" 图集信息 " + atlasSpritePathDic.Count + "  图集 " + spriteAtlasDic.Count);
            foreach (var item in atlasSpritePathDic)
            {
                Texture2D[] textures = new Texture2D[item.Value.Count];
                for (int i = 0; i < item.Value.Count; i++)
                {
                    textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(item.Value[i]);
                }
                SpriteAtlas atlas;
                if (!spriteAtlasDic.TryGetValue(item.Key, out atlas))
                {
                    atlas = new SpriteAtlas();
                    atlas.name = item.Key;
                    SpriteAtlasImportSetting(atlas);
                    string atlasPath = Path.Combine(EditorPathUtility.SpriteAtlasExportDirectory, item.Key + ".spriteatlas");
                    Debug.Log("创建图集" + atlasPath);
                    AssetDatabase.CreateAsset(atlas, atlasPath);
                    spriteAtlasDic.Add(item.Key, atlas);
                }
                atlas.Add(textures);
                EditorUtility.SetDirty(atlas);
                progress++;
                yield return null;
            }
            AssetDatabase.Refresh();
            Debug.Log("图集收集完成");
            bIsCollectPath = false;
            EditorUtility.ClearProgressBar();
            Close();
        }

        void SpriteAtlasImportSetting(SpriteAtlas atlas)
        {
            atlas.SetIncludeInBuild(false);
            atlas.SetIsVariant(false);
            SpriteAtlasPackingSettings packingSettings = atlas.GetPackingSettings();
            packingSettings.enableRotation = false;
            packingSettings.enableTightPacking = false;
            atlas.SetPackingSettings(packingSettings);
            SpriteAtlasTextureSettings texSetting = atlas.GetTextureSettings();
            texSetting.readable = false;
            texSetting.generateMipMaps = false;

            TextureImporterPlatformSettings androidSetting = atlas.GetPlatformSettings(BuildTarget.Android.ToString());
            androidSetting.overridden = true;
            androidSetting.format = TextureImporterFormat.ASTC_4x4;
            atlas.SetPlatformSettings(androidSetting);
            
            TextureImporterPlatformSettings iosSetting = atlas.GetPlatformSettings(BuildTarget.iOS.ToString());
            iosSetting.overridden = true;
            iosSetting.format = TextureImporterFormat.ASTC_4x4;
            atlas.SetPlatformSettings(iosSetting);
        }
    }
}