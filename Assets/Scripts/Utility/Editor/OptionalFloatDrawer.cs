using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomPropertyDrawer(typeof(OptionalFloat))]
public class OptionalFloatDrawer : PropertyDrawer {

    private const float ToggleWidth = 15;
    private const float PaddingWidth = 1;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var toggleRect = new Rect(position.x, position.y, ToggleWidth, position.height);
        var valueRect = new Rect(position.x + ToggleWidth + PaddingWidth, position.y, position.width - ToggleWidth - PaddingWidth, position.height);

        var enabledProp = property.FindPropertyRelative("enabled");
        EditorGUI.PropertyField(toggleRect, enabledProp, GUIContent.none);
        if (enabledProp.boolValue) {
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);
        }

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
