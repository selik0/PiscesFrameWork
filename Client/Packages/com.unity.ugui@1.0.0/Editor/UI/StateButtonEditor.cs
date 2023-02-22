using System.Collections.Generic;
/****************
 *@class name:		StateButton
 *@description:		状态按钮的Inspector面板
 *@author:			selik0
 *@date:			2023-02-05 20:19:04
 *@version: 		V1.0.0
*************************************************************************/
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor.UI
{
    [CustomEditor(typeof(StateButton), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the StateButton Component.
    ///   Extend this class to write a custom editor for a component derived from StateButton.
    /// </summary>
    public class StateButtonEditor : SelectableEditor
    {
        SerializedProperty m_OnClickProperty;

        ReorderableList m_StateDataList;

        Dictionary<string, ReorderableList> innerReorderableListDic = new Dictionary<string, ReorderableList>();
        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");

            m_StateDataList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_StateDataList"), true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "State List");
                },

                drawElementCallback = (Rect rect, int index, bool select, bool focused) =>
                {
                    SerializedProperty element = m_StateDataList.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("state"));
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 4, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("statusDesc"));

                    ReorderableList innerGraphicsReorderableList;
                    var m_StateGraphicsProperty = element.FindPropertyRelative("graphics");
                    if (innerReorderableListDic.ContainsKey(element.propertyPath))
                        innerGraphicsReorderableList = innerReorderableListDic[element.propertyPath];
                    else
                    {
                        innerGraphicsReorderableList = new ReorderableList(element.serializedObject, m_StateGraphicsProperty)
                        {
                            drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "State Control Graphics"); },

                            drawElementCallback = (Rect rect, int index, bool select, bool focused) =>
                            {
                                SerializedProperty element = m_StateGraphicsProperty.GetArrayElementAtIndex(index);

                                EditorGUI.PropertyField(rect, element);
                            },
                        };
                        innerReorderableListDic[element.propertyPath] = innerGraphicsReorderableList;
                    }

                    var height = (m_StateGraphicsProperty.arraySize + 3) * EditorGUIUtility.singleLineHeight;
                    innerGraphicsReorderableList.DoList(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2 + 8, rect.width, height));
                },
                onAddCallback = (ReorderableList list) =>
                {
                    if (list.serializedProperty != null)
                    {
                        list.serializedProperty.arraySize++;
                        list.index = list.serializedProperty.arraySize - 1;
                        var newItem = list.serializedProperty.GetArrayElementAtIndex(list.index);
                        var lastItem = list.serializedProperty.GetArrayElementAtIndex(list.index - 1);
                        newItem.FindPropertyRelative("state").intValue = lastItem.FindPropertyRelative("state").intValue + 1;
                    }
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    if (list.count <= 2)
                        EditorUtility.DisplayDialog("状态按钮状态删除提示", "状态列表至少需要2种状态", "确定");
                    else
                        ReorderableList.defaultBehaviours.DoRemoveButton(list);
                },
                elementHeightCallback = (int index) =>
                {
                    var element = m_StateDataList.serializedProperty.GetArrayElementAtIndex(index);
                    var graphics = element.FindPropertyRelative("graphics");
                    return (graphics.arraySize + 6) * EditorGUIUtility.singleLineHeight + 8;
                },
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OnClickProperty);

            EditorGUILayout.Space();

            m_StateDataList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}