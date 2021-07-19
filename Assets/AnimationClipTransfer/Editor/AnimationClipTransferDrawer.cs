using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// IngredientDrawer
[CustomPropertyDrawer(typeof(AnimationClipTransferBehaviour))]
public class AnimationClipTransferDrawer : PropertyDrawer
{
    private SerializedProperty prefabsProperty;
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 4;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
        // SerializedProperty newBehaviourVariableProp = property.FindPropertyRelative("Template");
        //
        // Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        // EditorGUI.PropertyField(singleFieldRect, newBehaviourVariableProp);
    
        
    }
    
    // public override VisualElement CreatePropertyGUI(SerializedProperty property)
    // {
        // Create property container element.
        // var container = new VisualElement();

        // // Create property fields.
        // var amountField = new PropertyField(property.FindPropertyRelative("amount"));
        // var unitField = new PropertyField(property.FindPropertyRelative("unit"));
        // var nameField = new PropertyField(property.FindPropertyRelative("name"), "Fancy Name");
        //
        // // Add fields to the container.
        // container.Add(amountField);
        // container.Add(unitField);
        // container.Add(nameField);

        // return container;
    // }
}
