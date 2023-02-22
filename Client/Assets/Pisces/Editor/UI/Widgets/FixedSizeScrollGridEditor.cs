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
    [CustomEditor(typeof(FixedSizeScrollGrid), true)]
    public class FixedSizeScrollGridEditor : AbstractScrollGridEditor
    {
        protected SerializedProperty m_IsAutoGroupElementCountProperty;
        protected SerializedProperty m_MinGroupElementCountProperty;
        Object elementPrfab;
        Vector2 elementSize = new Vector2(100, 100);
        protected override void OnEnable()
        {
            base.OnEnable();

            m_IsAutoGroupElementCountProperty = serializedObject.FindProperty("isAutoGroupElementCount");
            m_MinGroupElementCountProperty = serializedObject.FindProperty("minGroupElementCount");

            if (m_ElementPrefabsProperty.arraySize > 0)
                elementPrfab = m_ElementPrefabsProperty.GetArrayElementAtIndex(0).objectReferenceValue;

            if (m_ElementSizesProperty.arraySize > 0)
                elementSize = m_ElementSizesProperty.GetArrayElementAtIndex(0).vector2Value;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            serializedObject.ApplyModifiedProperties();
        }

        public override void DrawGroupElementCount()
        {
            EditorGUILayout.PropertyField(m_IsAutoGroupElementCountProperty);
            if (EditorGUILayout.BeginFadeGroup(m_IsAutoGroupElementCountProperty.boolValue ? 0 : 1))
            {
                EditorGUI.indentLevel++;
                base.DrawGroupElementCount();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(m_IsAutoGroupElementCountProperty.boolValue ? 1 : 0))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_MinGroupElementCountProperty);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
        }

        public override void DrawElementPrefabs()
        {
            EditorGUI.BeginChangeCheck();
            elementPrfab = EditorGUILayout.ObjectField("ElementPrefab", elementPrfab, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck())
            {
                m_ElementPrefabsProperty.arraySize = 1;
                SerializedProperty element = m_ElementPrefabsProperty.GetArrayElementAtIndex(0);
                element.objectReferenceValue = elementPrfab;
            }
        }

        public override void DrawElementSizes()
        {
            EditorGUI.BeginChangeCheck();
            elementSize = EditorGUILayout.Vector2Field("ElementSize", elementSize);
            if (EditorGUI.EndChangeCheck())
            {
                m_ElementSizesProperty.arraySize = 1;
                SerializedProperty element = m_ElementSizesProperty.GetArrayElementAtIndex(0);
                element.vector2Value = elementSize;
            }
        }
    }
}