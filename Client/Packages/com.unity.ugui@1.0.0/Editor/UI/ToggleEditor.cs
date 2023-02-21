using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(Toggle), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Toggle Component.
    /// Extend this class to write a custom editor for a component derived from Toggle.
    /// </summary>
    public class ToggleEditor : SelectableEditor
    {
        SerializedProperty m_OnValueChangedProperty;
        SerializedProperty m_TransitionProperty;
        SerializedProperty m_GraphicProperty;
        SerializedProperty m_GroupProperty;
        SerializedProperty m_IsOnProperty;
        SerializedProperty m_BackgroundGraphicsProperty;
        SerializedProperty m_CheckmarkGraphicsProperty;

        GUIContent m_BackgroundCollect = EditorGUIUtility.TrTextContent("Collect Background Graphics", "收集Target Graphic下所有的Graphics");
        GUIContent m_CheckmarkCollect = EditorGUIUtility.TrTextContent("Collect Checkmark Graphics", "收集Graphic下所有的Graphics");
        UnityEditorInternal.ReorderableList m_BackgroudGraphicsList;
        UnityEditorInternal.ReorderableList m_CheckmarkGraphicsList;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_TransitionProperty = serializedObject.FindProperty("toggleTransition");
            m_GraphicProperty = serializedObject.FindProperty("graphic");
            m_GroupProperty = serializedObject.FindProperty("m_Group");
            m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
            m_BackgroundGraphicsProperty = serializedObject.FindProperty("backgroundGraphics");
            m_CheckmarkGraphicsProperty = serializedObject.FindProperty("checkmarkGraphics");
            m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");

            m_BackgroudGraphicsList = new UnityEditorInternal.ReorderableList(serializedObject, m_BackgroundGraphicsProperty)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Background Graphics");
                    if (GUI.Button(new Rect(rect.x + 150, rect.y, 200, rect.height), new GUIContent(m_BackgroundCollect)))
                    {
                        Toggle toggle = serializedObject.targetObject as Toggle;
                        toggle.CollectBackgroundGraphics();
                        EditorUtility.SetDirty(toggle);
                    }
                },
                drawElementCallback = (Rect rect, int index, bool select, bool focused) =>
                {
                    SerializedProperty element = m_BackgroundGraphicsProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element);
                }
            };

            m_CheckmarkGraphicsList = new UnityEditorInternal.ReorderableList(serializedObject, m_CheckmarkGraphicsProperty)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Chackmark Graphics");
                    if (GUI.Button(new Rect(rect.x + 150, rect.y, 200, rect.height), new GUIContent(m_CheckmarkCollect)))
                    {
                        Toggle toggle = serializedObject.targetObject as Toggle;
                        toggle.CollectCheckmarkGraphics();
                        EditorUtility.SetDirty(toggle);
                    }
                },
                drawElementCallback = (Rect rect, int index, bool select, bool focused) =>
                {
                    SerializedProperty element = m_CheckmarkGraphicsProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            m_BackgroudGraphicsList.DoLayoutList();
            m_CheckmarkGraphicsList.DoLayoutList();
            EditorGUILayout.Space();

            serializedObject.Update();
            Toggle toggle = serializedObject.targetObject as Toggle;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_IsOnProperty);
            if (EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(toggle.gameObject.scene);
                ToggleGroup group = m_GroupProperty.objectReferenceValue as ToggleGroup;

                toggle.isOn = m_IsOnProperty.boolValue;

                if (group != null && group.isActiveAndEnabled && toggle.IsActive())
                {
                    if (toggle.isOn || (!group.AnyTogglesOn() && !group.allowSwitchOff))
                    {
                        toggle.isOn = true;
                        group.NotifyToggleOn(toggle);
                    }
                }
            }
            EditorGUILayout.PropertyField(m_TransitionProperty);
            EditorGUILayout.PropertyField(m_GraphicProperty);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_GroupProperty);
            if (EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(toggle.gameObject.scene);
                ToggleGroup group = m_GroupProperty.objectReferenceValue as ToggleGroup;
                toggle.group = group;
            }

            EditorGUILayout.Space();

            // Draw the event notification options
            EditorGUILayout.PropertyField(m_OnValueChangedProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
