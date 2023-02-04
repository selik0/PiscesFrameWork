
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pisces;
using UnityEditorInternal;
using UnityEngine.UI;
using System.Linq;
namespace PiscesEditor
{
    [CustomEditor(typeof(Bind))]
    public class BindEditor : Editor
    {
        protected Bind mBind;
        protected List<string> m_repeatItemKey;
        protected ReorderableList m_uiList;
        protected SerializedProperty m_widgetsProperty;
        protected List<Bind.BindObject> itemlist = new List<Bind.BindObject>();
        protected Bind.BindObject m_removeItem;


        // 节点命名 btn_Save 表示需要绑定这个节点的Button, btn_img_Save 表示需要Button和Image组件，以此类推
        protected Dictionary<string, List<System.Type>> objectTypeList = new Dictionary<string, List<System.Type>>(){
            {"btn", new List<System.Type>(){ typeof(Button)}},
            {"dropdown", new List<System.Type>(){typeof(Dropdown)}},
            {"img", new List<System.Type>(){typeof(MyImage)}},
            {"input", new List<System.Type>(){typeof(InputField)}},
            {"raw", new List<System.Type>(){typeof(RawImage)}},
            {"tog", new List<System.Type>(){typeof(Toggle)}},
            {"txt", new List<System.Type>(){typeof(Text), typeof(TMPro.TextMeshProUGUI), typeof(TMPro.TextMeshPro)}},
            {"slider", new List<System.Type>(){typeof(Slider)}},
            {"scroll", new List<System.Type>(){typeof(ScrollRect)}},
            {"trans", new List<System.Type>(){typeof(RectTransform), typeof(Transform)}},
        };

        private void OnEnable()
        {
            m_repeatItemKey = new List<string>();
            mBind = target as Bind;
            m_widgetsProperty = serializedObject.FindProperty("objects");
            m_uiList = new ReorderableList(serializedObject, m_widgetsProperty, true, true, false, false);
            m_uiList.drawElementCallback = DrawNameElement;
            m_uiList.elementHeight = 62;

            m_uiList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Bind List");
            };

            m_uiList.onChangedCallback = (ReorderableList list) =>
            {
                SetKey();
                ValidateItems();
            };

            ValidateItems();
        }

        public override void OnInspectorGUI()
        {
            itemlist.Clear();
            if (mBind.objects == null)
                mBind.objects = new Bind.BindObject[0];
            if (mBind.objects != null)
                itemlist.AddRange(mBind.objects);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                Bind.BindObject tmpItem = new Bind.BindObject();
                // itemlist.Insert(0, tmpItem);
                itemlist.Add(tmpItem);
            }
            GUILayout.Space(10);
            SetBeginFunction();
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            m_uiList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();

            mBind.objects = itemlist.ToArray();

            if (GUI.changed)
            {
                SetKey();
                ValidateItems();
                UnityEditor.EditorUtility.SetDirty(mBind.gameObject);
            }

            if (m_removeItem != null)
            {
                itemlist.Remove(m_removeItem);
                m_removeItem = null;
                mBind.objects = itemlist.ToArray();
                ValidateItems();
            }
            mBind.objects = itemlist.ToArray();
            GUILayout.Space(10);
            SetEndFunction();
        }

        protected virtual void SetBeginFunction()
        {
            if (GUILayout.Button("一键收集", GUILayout.Width(80)))
            {
                OneKeyCollected(mBind.gameObject);
                Sort();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("排序", GUILayout.Width(80)))
            {
                Sort();
            }
            GUILayout.Space(10);
        }

        protected virtual void SetEndFunction()
        {

        }

        void DrawNameElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty itemData = m_uiList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty itemKeyProperty = itemData.FindPropertyRelative("key");
            SerializedProperty itemTypeProperty = itemData.FindPropertyRelative("type");
            DrawIsValidate(rect, itemKeyProperty);
            //key
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, 18), "key:");
            itemKeyProperty.stringValue = EditorGUI.TextField(new Rect(rect.x + 60, rect.y, 100, 18), itemKeyProperty.stringValue);
            //type
            EditorGUI.LabelField(new Rect(rect.x + 170, rect.y, 50, 18), "type:");
            Bind.BindObjectType type = (Bind.BindObjectType)EditorGUI.EnumPopup(new Rect(rect.x + 220, rect.y, rect.width - 170, 18), (Bind.BindObjectType)itemTypeProperty.enumValueIndex);
            itemTypeProperty.enumValueIndex = (int)type;
            //value
            switch (type)
            {
                case Bind.BindObjectType.Object:
                    SerializedProperty itemSourceProperty = itemData.FindPropertyRelative("obj");
                    UnityEngine.Object itemSourceObj = itemSourceProperty.objectReferenceValue;
                    //EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 50, 18), "object:");
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 50, 18), new GUIContent("object:", GetWidgetType(itemKeyProperty.stringValue, itemSourceObj)));
                    EditorGUI.ObjectField(new Rect(rect.x + 60, rect.y + 20, rect.width - 60, 18), itemSourceProperty, new GUIContent());
                    if (itemSourceObj != null)
                    {
                        GUI.Label(new Rect(rect.x, rect.y + 40, 200, 18), "<" + itemSourceObj.GetType().ToString().Replace("UnityEngine.", "") + ">");
                        if (GUI.Button(new Rect(rect.width - 150, rect.y + 40, 60, 18), "快速"))
                        {
                            FastChangeComponent(itemSourceObj, itemSourceProperty);
                        }
                        if (GUI.Button(new Rect(rect.width - 88, rect.y + 40, 60, 18), "切换"))
                        {
                            ChangeComponent(itemSourceObj, itemSourceProperty);
                        }
                    }
                    break;
                case Bind.BindObjectType.String:
                    SerializedProperty itemStrProperty = itemData.FindPropertyRelative("str");
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 50, 18), "string:");
                    itemStrProperty.stringValue = EditorGUI.TextField(new Rect(rect.x + 60, rect.y + 20, rect.width - 60, 18), itemStrProperty.stringValue);
                    break;
                case Bind.BindObjectType.Boolean:
                    SerializedProperty itemBoolProperty = itemData.FindPropertyRelative("b");
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 50, 18), "bool:");
                    itemBoolProperty.boolValue = EditorGUI.Toggle(new Rect(rect.x + 60, rect.y + 20, rect.width - 60, 18), itemBoolProperty.boolValue);
                    break;
                case Bind.BindObjectType.Integer:
                    SerializedProperty itemIntProperty = itemData.FindPropertyRelative("i");
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 50, 18), "int:");
                    itemIntProperty.intValue = EditorGUI.IntField(new Rect(rect.x + 60, rect.y + 20, rect.width - 60, 18), itemIntProperty.intValue);
                    break;
                case Bind.BindObjectType.Float:
                    SerializedProperty itemFloatProperty = itemData.FindPropertyRelative("f");
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 50, 18), "float:");
                    itemFloatProperty.floatValue = EditorGUI.FloatField(new Rect(rect.x + 60, rect.y + 20, rect.width - 60, 18), itemFloatProperty.floatValue);
                    break;
            }

            if (GUI.Button(new Rect(rect.width - 25, rect.y + 40, 60, 18), "移除"))
            {
                m_removeItem = mBind.objects[index];
            }
        }

        void DrawIsValidate(Rect rect, SerializedProperty itemKeyProperty)
        {
            Texture2D tex = EditorGUIUtility.whiteTexture;
            if (m_repeatItemKey.Contains(itemKeyProperty.stringValue))
            {
                GUI.color = Color.red;
            }
            else
            {
                GUI.color = new Color(0f, 0f, 0f, 0.25f);
            }
            GUI.DrawTexture(new Rect(rect.x - 20, rect.y, rect.width + 20, 80), tex);
            GUI.color = Color.white;
        }

        void SetKey()
        {
            Bind.BindObject[] objects = mBind.objects;
            if (objects == null)
                return;

            for (int i = 0; i < objects.Length; i++)
            {
                Bind.BindObject item = objects[i];
                if (item != null && string.IsNullOrEmpty(item.key))
                {
                    SetItemKey(item);
                }
            }
        }

        protected void SetItemKey(Bind.BindObject item)
        {
            switch (item.type)
            {
                case Bind.BindObjectType.Object:
                    if (item.obj != null)
                    {
                        item.key = item.obj.name.Replace(" ", "");
                    }
                    break;
                default:
                    item.key = GetKeyByObjectType(item.type);
                    break;
            }
        }

        void ValidateItems()
        {
            Bind.BindObject[] objects = mBind.objects;
            if (objects == null)
                return;

            m_repeatItemKey.Clear();
            for (int i = 0; i < objects.Length; i++)
            {
                for (int j = i + 1; j < objects.Length; j++)
                {
                    if (objects[i].key == objects[j].key)
                    {
                        m_repeatItemKey.Add(objects[i].key);
                    }
                }
            }
        }

        static string GetKeyByObjectType(Bind.BindObjectType type)
        {
            string key = null;
            switch (type)
            {
                case Bind.BindObjectType.Boolean:
                    key = "boolValue";
                    break;
                case Bind.BindObjectType.Float:
                    key = "floatValue";
                    break;
                case Bind.BindObjectType.Integer:
                    key = "intValue";
                    break;
                case Bind.BindObjectType.String:
                    key = "strValue";
                    break;
            }
            return key;
        }

        void OneKeyCollected(GameObject go)
        {
            // 先
            Dictionary<Object, Bind.BindObject> _objDic = new Dictionary<Object, Bind.BindObject>();
            foreach (Bind.BindObject item in itemlist)
                _objDic.Add(item.obj, item);

            RectTransform[] allRectTransform = go.GetComponentsInChildren<RectTransform>(true);
            foreach (var nodeRectTransform in allRectTransform)
            {
                string[] names = nodeRectTransform.name.Split('_');
                if (names.Length >= 2)
                {
                    for (var i = 0; i < names.Length - 1; i++)
                    {
                        string endName = names[i] + names[names.Length - 1];
                        List<System.Type> list;
                        if (objectTypeList.TryGetValue(names[i], out list))
                        {
                            foreach (System.Type type in list)
                            {
                                var obj = nodeRectTransform.GetComponent(type);
                                if (obj != null)
                                {
                                    if (_objDic.Keys.Contains(obj))
                                    {
                                        _objDic[obj].key = endName;
                                        _objDic[obj].obj = obj;
                                    }
                                    else
                                    {
                                        Bind.BindObject tmpItem = new Bind.BindObject();
                                        tmpItem.key = endName;
                                        tmpItem.obj = obj;
                                        itemlist.Add(tmpItem);
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("不支持该类型缩写" + names[i]);
                        }
                    }
                }
            }
        }

        void FastChangeComponent(UnityEngine.Object itemSourceObj, SerializedProperty itemSourceProperty)
        {
            GameObject go = null;
            if (itemSourceObj is GameObject)
                go = itemSourceObj as GameObject;
            else
                go = (itemSourceObj as Component).gameObject;
            foreach (List<System.Type> types in objectTypeList.Values)
            {
                UnityEngine.Object obj = null;
                foreach (System.Type type in types)
                {
                    obj = go.GetComponent(type);
                    if (obj != null)
                    {
                        itemSourceProperty.objectReferenceValue = obj;
                        break;
                    }
                }
                if (obj != null)
                    break;
            }
        }

        static void ChangeComponent(UnityEngine.Object itemSourceObj, SerializedProperty itemSourceProperty)
        {
            GameObject go = null;
            if (itemSourceObj is GameObject)
                go = itemSourceObj as GameObject;
            else
                go = (itemSourceObj as Component).gameObject;

            Component[] cps = null;
            cps = go.GetComponents<Component>();

            List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
            objs.AddRange(cps);
            objs.Add(cps[0].gameObject);

            int cpIndex = -99;
            int itemInstanceID = itemSourceObj.GetInstanceID();
            for (int k = 0; k < objs.Count; k++)
            {
                if (itemInstanceID == objs[k].GetInstanceID())
                {
                    cpIndex = k;
                    break;
                }
            }
            cpIndex++;
            if (cpIndex >= objs.Count)
            {
                itemSourceProperty.objectReferenceValue = objs[0];
            }
            else
            {
                itemSourceProperty.objectReferenceValue = objs[cpIndex];
            }
        }

        public void Sort()
        {
            itemlist.Sort((x, y) =>
            {
                return x.key.CompareTo(y.key);
            });
        }

        protected virtual string GetWidgetType(string key, UnityEngine.Object obj)
        {
            if (obj != null)
                return obj.GetType().ToString();
            return "unknow";
        }
    }
}
