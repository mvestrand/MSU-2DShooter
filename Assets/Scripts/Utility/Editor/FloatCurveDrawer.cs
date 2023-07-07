using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomPropertyDrawer(typeof(FloatCurve))]
public class FloatCurveDrawer : PropertyDrawer {

    private const float ModeWidth = 20;
    private const float PaddingWidth = 1;
    private const float ScaleLabelWidth = 38;
    private const float ValueWidth = 60;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var modeRect = new Rect(position.x, position.y, ModeWidth, position.height);
    
        var modeProp = property.FindPropertyRelative("mode");
        EditorGUI.PropertyField(modeRect, modeProp, GUIContent.none);


        if (modeProp.intValue == (int)FloatCurveMode.Constant) {
            float usedWidth = ModeWidth + PaddingWidth;
            var valueRect = new Rect(position.x + usedWidth, position.y, position.width - usedWidth, position.height);
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

        } else if (modeProp.intValue == (int)FloatCurveMode.Curve) {
            float usedWidth = ModeWidth + PaddingWidth;
            var curveRect = new Rect(position.x + usedWidth, position.y, position.width - usedWidth, position.height);

            DrawCurveFields(curveRect, property);

        } else if (modeProp.intValue == (int)FloatCurveMode.CurvePlusConstant) {
            float usedWidth = ModeWidth + PaddingWidth;

            var valueRect = new Rect(position.x + usedWidth, position.y, ValueWidth, position.height);
            usedWidth += ValueWidth + PaddingWidth;
            var curveRect = new Rect(position.x + usedWidth, position.y, position.width - usedWidth, position.height);

            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);
            DrawCurveFields(curveRect, property);

        } else if (modeProp.intValue == (int)FloatCurveMode.BetweenTwoConstants) {

            float usedWidth = ModeWidth + PaddingWidth;
            float halfWidth = (position.width - usedWidth) / 2f;
            float labelWidth = 40f;

            var xLabelRect = new Rect(position.x + usedWidth, position.y, labelWidth, position.height);
            var xRect = new Rect(position.x + usedWidth + labelWidth, position.y, halfWidth-labelWidth, position.height);
            var yLabelRect = new Rect(position.x + usedWidth + halfWidth, position.y, labelWidth, position.height);
            var yRect = new Rect(position.x + usedWidth + halfWidth+labelWidth, position.y, halfWidth-labelWidth, position.height);

            EditorGUI.LabelField(xLabelRect, "    min:");
            EditorGUI.PropertyField(xRect, property.FindPropertyRelative("min"), GUIContent.none);

            EditorGUI.LabelField(yLabelRect, "    max:");
            EditorGUI.PropertyField(yRect, property.FindPropertyRelative("max"), GUIContent.none);
            
        }

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    private void DrawCurveFields(Rect position, SerializedProperty property) {
        var scaleLabelRect = new Rect(position.x, position.y, ScaleLabelWidth, position.height);
        float usedWidth = ScaleLabelWidth + PaddingWidth;

        var scaleRect = new Rect(position.x + usedWidth, position.y, ValueWidth, position.height);
        usedWidth += ValueWidth + PaddingWidth;

        var curveRect = new Rect(position.x + usedWidth, position.y, position.width - usedWidth, position.height);

        EditorGUI.LabelField(scaleLabelRect, " scale:");
        EditorGUI.PropertyField(scaleRect, property.FindPropertyRelative("curveScale"), GUIContent.none);
        EditorGUI.PropertyField(curveRect, property.FindPropertyRelative("curve"), GUIContent.none);
    }

}
