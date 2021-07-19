using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EditorTest2Behaviour))]
public class EditorTest2Drawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 2;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty AnimationClipProp = property.FindPropertyRelative("AnimationClip");
        SerializedProperty LocalPostionProp = property.FindPropertyRelative("LocalPostion");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, AnimationClipProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, LocalPostionProp);
    }
}
