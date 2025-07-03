using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(SurfaceBoolPair))]
public class SurfaceBoolPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var surfaceProp = property.FindPropertyRelative("Surface");
        var isActiveProp = property.FindPropertyRelative("IsActive");

        // Draw the surface name as a label (read-only)
        var surfaceName = surfaceProp.FindPropertyRelative("Name").stringValue;
        var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        EditorGUI.LabelField(labelRect, surfaceName);

        // Draw the boolean toggle only
        var toggleRect = new Rect(position.x + EditorGUIUtility.labelWidth + 10, position.y, 20, position.height);
        isActiveProp.boolValue = EditorGUI.Toggle(toggleRect, isActiveProp.boolValue);

        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties(); // Ensure changes are saved
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + 2; // Adjust height for the two fields
    }
}
[CustomPropertyDrawer(typeof(SurfaceTag))]
public class SurfaceTagPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var nameProp = property.FindPropertyRelative("Name");
        var priceProp = property.FindPropertyRelative("Price");
        // Draw the name field
        var nameRect = new Rect(position.x, position.y, position.width - 50, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
        // Draw the price field
        var priceRect = new Rect(position.x + position.width - 45, position.y, 40, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(priceRect, priceProp, GUIContent.none);
        EditorGUI.EndProperty();
    }
}