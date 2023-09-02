using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

// [CustomPropertyDrawer(typeof(FireSequenceFrame))]
// public class FireSequenceFrameDrawer : PropertyDrawer {

//     private const float TimeFieldWidth = 40f;
//     private const float PaddingWidth = 1f;
//     private const float ModeFieldWidth = 20f;
//     private const float EffectFieldWidth = 80f;

//     private bool isExpanded = false;

//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        

//         EditorGUI.BeginProperty(position, label, property);

//         var indent = EditorGUI.indentLevel;
//         EditorGUI.indentLevel = 0;

//         // First line
//         var timeRect = new Rect(position.x, position.y, TimeFieldWidth, EditorGUIUtility.singleLineHeight);
//         var usedWidth = TimeFieldWidth + PaddingWidth;
//         var modeRect = new Rect(position.x + usedWidth, position.y, ModeFieldWidth, EditorGUIUtility.singleLineHeight);
//         usedWidth += ModeFieldWidth + PaddingWidth;
//         var effectRect = new Rect(position.x + usedWidth, position.y, EffectFieldWidth, EditorGUIUtility.singleLineHeight);
//         usedWidth += EffectFieldWidth + PaddingWidth;
//         var valueRect = new Rect(position.x + usedWidth, position.y, position.width - usedWidth, EditorGUIUtility.singleLineHeight);

//         var modeProp = property.FindPropertyRelative("mode");

//         EditorGUI.PropertyField(timeRect, property.FindPropertyRelative("time"), GUIContent.none);
//         EditorGUI.PropertyField(modeRect, modeProp, GUIContent.none);
//         EditorGUI.PropertyField(effectRect, property.FindPropertyRelative("effect"), GUIContent.none);
//         if (modeProp.intValue == (int)FireSequenceFrameMode.Projectile) {
//             EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("projectile"), GUIContent.none);
//         } else if (modeProp.intValue == (int)FireSequenceFrameMode.Pattern) {
//             EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("pattern"), GUIContent.none);
//         } else if (modeProp.intValue == (int)FireSequenceFrameMode.Pattern) {

//         }


//         EditorGUI.indentLevel = indent;
//         EditorGUI.EndProperty();
//     }

//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         return base.GetPropertyHeight(property, label);
//     }

// } 


// [CustomEditor(typeof(FireSequence))]
// public class FireSequenceEditor : Editor {

//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();

//     }
// }
