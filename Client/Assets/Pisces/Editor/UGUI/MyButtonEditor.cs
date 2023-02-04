/****************
 *@class name:		MyButtonEditor,cs
 *@description:		
 *@author:			selik0
 *@date:			2023-02-02 14:13:02
 *@version: 		V1.0.0
*************************************************************************/

using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(MyButton), true)]
    [CanEditMultipleObjects]
    public class MyButtonEditor : MySelectableEditor
    {
        SerializedProperty m_IsOpenIntervalProperty;
        SerializedProperty m_IsOpenLongPressProperty;
        SerializedProperty m_IntervalTimeProperty;
        SerializedProperty m_LongPressTimeProperty;
        SerializedProperty m_OnClickProperty;
        SerializedProperty m_OnLongPressProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_IsOpenIntervalProperty = serializedObject.FindProperty("m_IsOpenClickInterval");
            m_IsOpenLongPressProperty = serializedObject.FindProperty("m_IsOpenLongPress");
            m_IntervalTimeProperty = serializedObject.FindProperty("m_ClickIntervalTime");
            m_LongPressTimeProperty = serializedObject.FindProperty("m_LongPressTime");
            m_OnLongPressProperty = serializedObject.FindProperty("m_OnLongPress");
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_IsOpenIntervalProperty);
            EditorGUILayout.PropertyField(m_IsOpenLongPressProperty);
            EditorGUILayout.PropertyField(m_IntervalTimeProperty);
            EditorGUILayout.PropertyField(m_LongPressTimeProperty);
            EditorGUILayout.PropertyField(m_OnLongPressProperty);
            EditorGUILayout.PropertyField(m_OnClickProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
