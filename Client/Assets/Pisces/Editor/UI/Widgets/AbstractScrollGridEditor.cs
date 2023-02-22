/****************
 *@class name:		FixedSizeScrollGridEditor
 *@description:		
 *@author:			selik0
 *@date:			2023-02-21 17:07:55
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;
namespace UnityEditor.UI
{
    [CustomEditor(typeof(AbstractScrollGrid), true)]
    public class AbstractScrollGridEditor : Editor
    {
        SerializedProperty m_HeadPaddingProperty;
        SerializedProperty m_TailPaddingProperty;
        SerializedProperty m_ElementSpacingProperty;
        SerializedProperty m_ElementChangeProperty;
        SerializedProperty m_GroupElementCountProperty;
        protected SerializedProperty m_ElementPrefabsProperty;
        protected SerializedProperty m_ElementSizesProperty;
        ReorderableList m_ElementPrefabsList;
        ReorderableList m_ElementSizesList;
        protected virtual void OnEnable()
        {
            m_HeadPaddingProperty = serializedObject.FindProperty("headPadding");
            m_TailPaddingProperty = serializedObject.FindProperty("tailPadding");
            m_GroupElementCountProperty = serializedObject.FindProperty("groupElementCount");
            m_ElementSpacingProperty = serializedObject.FindProperty("elementSpacing");
            m_ElementPrefabsProperty = serializedObject.FindProperty("m_ElementPrefabs");
            m_ElementSizesProperty = serializedObject.FindProperty("elementSizes");
            m_ElementChangeProperty = serializedObject.FindProperty("m_OnElementChange");

            InitializeElementPrefabList();
            InitializeElementSizeList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawGroupElementCount();
            EditorGUILayout.PropertyField(m_HeadPaddingProperty);
            EditorGUILayout.PropertyField(m_TailPaddingProperty);
            EditorGUILayout.PropertyField(m_ElementSpacingProperty);

            DrawElementPrefabs();
            DrawElementSizes();
            DrawOnElementChagne();
            serializedObject.ApplyModifiedProperties();
        }

        void InitializeElementPrefabList()
        {
            m_ElementPrefabsList = new ReorderableList(serializedObject, m_ElementPrefabsProperty, false, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "ElementPrefabs"); },
                drawElementCallback = (Rect rect, int index, bool select, bool focused) =>
                {
                    SerializedProperty element = m_ElementPrefabsProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element);
                },
                onAddCallback = (ReorderableList list) =>
                {
                    ReorderableList.defaultBehaviours.DoAddButton(m_ElementSizesList);
                    ReorderableList.defaultBehaviours.DoAddButton(list);
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    m_ElementSizesList.index = m_ElementPrefabsList.index;
                    ReorderableList.defaultBehaviours.DoRemoveButton(m_ElementSizesList);
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
        }

        void InitializeElementSizeList()
        {
            m_ElementSizesList = new ReorderableList(serializedObject, m_ElementSizesProperty, false, true, false, false)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "ElementSizes"); },
                drawElementCallback = (Rect rect, int index, bool select, bool focused) =>
                {
                    SerializedProperty element = m_ElementSizesProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element);
                },
            };
        }

        public virtual void DrawGroupElementCount()
        {
            EditorGUILayout.PropertyField(m_GroupElementCountProperty);
        }

        public virtual void DrawElementPrefabs()
        {
            EditorGUILayout.Space(15);
            for (int i = 0; i < m_ElementPrefabsProperty.arraySize; i++)
            {
                if (m_ElementPrefabsProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("ElementPrefab 不能为空.", MessageType.Warning);
                    break;
                }
            }
            m_ElementPrefabsList.DoLayoutList();
        }

        public virtual void DrawElementSizes()
        {
            m_ElementSizesList.DoLayoutList();
        }

        public void DrawOnElementChagne()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_ElementChangeProperty);
        }
    }
}