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
        SerializedProperty m_GroupElementCountProperty;
        SerializedProperty m_ElementSpacingProperty;
        SerializedProperty m_ElementChangeProperty;
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
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_GroupElementCountProperty);
            EditorGUILayout.PropertyField(m_HeadPaddingProperty);
            EditorGUILayout.PropertyField(m_TailPaddingProperty);
            EditorGUILayout.PropertyField(m_ElementSpacingProperty);

            DrawElementPrefabs();
            DrawElementSizes();
            DrawOnElementChagne();
            serializedObject.ApplyModifiedProperties();
        }

        public virtual void DrawElementPrefabs()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_ElementPrefabsProperty);
            if (EditorGUI.EndChangeCheck())
                m_ElementSizesProperty.arraySize = m_ElementPrefabsProperty.arraySize;

            // m_ElementPrefabsList = new ReorderableList(serializedObject, m_ElementPrefabsProperty, true, false, true, true)
            // {
            //     drawElementCallback = (Rect rect, int index, bool select, bool focused) =>
            //     {
            //         SerializedProperty element = m_ElementPrefabsList.serializedProperty.GetArrayElementAtIndex(index);
            //         EditorGUI.BeginChangeCheck();
            //         EditorGUILayout.PropertyField(element);
            //         if (EditorGUI.EndChangeCheck())
            //         {

            //         }

            //     }
            // };
        }

        public virtual void DrawElementSizes()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_ElementSizesProperty);
            if (EditorGUI.EndChangeCheck())
                m_ElementPrefabsProperty.arraySize = m_ElementSizesProperty.arraySize;
        }

        public void DrawOnElementChagne()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_ElementChangeProperty);
        }
    }
}