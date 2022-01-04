using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using MVest;

// [CustomPropertyDrawer(typeof(DefaultTo<,>), true)]
// public class ScriptableReferenceDrawer : PropertyDrawer {

//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//         EditorGUI.BeginProperty(position, label, property);

//         // Draw property label
//         position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//         var isSetRect = new Rect(position.x, position.y, position.height, position.height);
//         var valueRect = new Rect(position.x + position.height + 2, position.y, position.width - (position.height + 2), position.height);

//         var useConstant = property.FindPropertyRelative("useConstant");

//         int index = (useConstant.boolValue ? 0 : 1);
//         index = EditorGUI.Popup(isConstRect, index, options);

//         useConstant.boolValue = (index == 0);


//         if (useConstant.boolValue)
//             EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("constantValue"), GUIContent.none);
//         else
//             EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("variable"), GUIContent.none);

//         EditorGUI.EndProperty();
//     }

// }