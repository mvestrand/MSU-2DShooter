using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using Sirenix.OdinInspector;

namespace MVest {

[CustomPropertyDrawer(typeof(ScriptableReference<,>), true)]
public class ScriptableReferenceDrawer : PropertyDrawer {

    private static string[] options = {"Constant", "Variable"};

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        // Draw property label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var isConstRect = new Rect(position.x, position.y, position.height, position.height);
        var valueRect = new Rect(position.x + position.height + 2, position.y, position.width - (position.height + 2), position.height);

        var useConstant = property.FindPropertyRelative("useConstant");

        int index = (useConstant.boolValue ? 0 : 1);
        index = EditorGUI.Popup(isConstRect, index, options);

        useConstant.boolValue = (index == 0);


        if (useConstant.boolValue)
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("constantValue"), GUIContent.none);
        else
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("variable"), GUIContent.none);

        EditorGUI.EndProperty();
    }

}


[CustomEditor(typeof(ScriptableVariable<>),true)]
public class ScriptableVariableEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var targetVar = (IScriptableVariableCallback)target;

        if (GUILayout.Button("Update")) {
            targetVar.UpdateListeners();
        }
    }
}

}
