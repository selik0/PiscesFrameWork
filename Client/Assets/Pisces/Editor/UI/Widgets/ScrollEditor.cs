/****************
 *@class name:		ScrollEditor
 *@description:		scroll的编辑器UI
 *@author:			selik0
 *@date:			2023-02-07 20:53:46
 *@version: 		V1.0.0
*************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(Scroll), true)]
    [CanEditMultipleObjects]
    public class ScrollEditor : Editor
    {
        SerializedProperty m_ContentSize;
        SerializedProperty m_Direction;
        SerializedProperty m_MovementType;
        SerializedProperty m_Elasticity;
        SerializedProperty m_Inertia;
        SerializedProperty m_DecelerationRate;
        SerializedProperty m_ScrollSensitivity;
        SerializedProperty m_Viewport;
        SerializedProperty m_OnValueChanged;
        AnimBool m_ShowElasticity;
        AnimBool m_ShowDecelerationRate;
        protected virtual void OnEnable()
        {
            m_ContentSize = serializedObject.FindProperty("m_ContentSize");
            m_Direction = serializedObject.FindProperty("direction");
            m_MovementType = serializedObject.FindProperty("movementType");
            m_Elasticity = serializedObject.FindProperty("elasticity");
            m_Inertia = serializedObject.FindProperty("inertia");
            m_DecelerationRate = serializedObject.FindProperty("decelerationRate");
            m_ScrollSensitivity = serializedObject.FindProperty("scrollSensitivity");
            m_Viewport = serializedObject.FindProperty("m_Viewport");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");

            m_ShowElasticity = new AnimBool(Repaint);
            m_ShowDecelerationRate = new AnimBool(Repaint);
            SetAnimBools(true);
        }

        protected virtual void OnDisable()
        {
            m_ShowElasticity.valueChanged.RemoveListener(Repaint);
            m_ShowDecelerationRate.valueChanged.RemoveListener(Repaint);
        }

        void SetAnimBools(bool instant)
        {
            SetAnimBool(m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int)Scroll.MovementType.Elastic, instant);
            SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue == true, instant);
        }

        void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }

        public override void OnInspectorGUI()
        {
            SetAnimBools(false);

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_ContentSize);

            EditorGUILayout.PropertyField(m_Direction);

            EditorGUILayout.PropertyField(m_MovementType);
            if (EditorGUILayout.BeginFadeGroup(m_ShowElasticity.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_Elasticity);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_Inertia);
            if (EditorGUILayout.BeginFadeGroup(m_ShowDecelerationRate.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_DecelerationRate);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_ScrollSensitivity);

            EditorGUILayout.Space();


            EditorGUILayout.PropertyField(m_Viewport);
            RectTransform rect = m_Viewport.objectReferenceValue as RectTransform;
            if (rect == null)
                EditorGUILayout.HelpBox("视图窗口不能为空", MessageType.Warning);
            else
                m_ContentSize.vector2Value = Vector2.Max(m_ContentSize.vector2Value, rect.rect.size);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_OnValueChanged);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
