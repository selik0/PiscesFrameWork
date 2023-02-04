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
    [CustomEditor(typeof(MyStateButton), true)]
    [CanEditMultipleObjects]
    public class MyStateButtonEditor : MySelectableEditor
    {

        SerializedProperty m_OnClickProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OnClickProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
