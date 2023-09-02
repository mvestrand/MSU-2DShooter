using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEditor;

[CustomPropertyDrawer(typeof(ProjectileModifiers))]
public class ProjectileModifiersDrawer : PropertyDrawer
{

    private static readonly Color darkenColor = new Color(0, 0, 0, 0.1f);
    private static readonly Color lightenColor = new Color(1, 1, 1, 0.1f);

    private Rect[] line1 = new Rect[4];
    private static float[] line1MinWidths = new float[4] { 100f, 80f, 65f, 40f };
    private static float[] line1Weights = new float[4] { 0, 500f, 0, 300f };
    private Rect[] line2 = new Rect[4];
    private static float[] line2MinWidths = new float[4] { 100f, 80f, 65f, 40f};
    private static float[] line2Weights = new float[4] { 0, 500f, 0, 300f};
    private Rect[] line3 = new Rect[7];
    private static float[] line3MinWidths = new float[7] { 140f, 20f, 50f, 80f, 80f, 50f, 20f};
    private static float[] line3Weights = new float[7] { 0, 1, 0, 1, 300f,0,1};

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.border = new RectOffset(1, 1, 1, 1);
        //labelStyle.padding = new RectOffset(0,0,0,0);
        GUIStyle sublabelStyle = new GUIStyle(EditorStyles.miniLabel);
        //sublabelStyle.padding = new RectOffset(0,0,0,0);

        

        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Initial modifiers
        EditorGUICustomUtility.ComputeRects(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            line1, line1MinWidths, line1Weights);

        EditorGUI.LabelField(line1[0], "Offset");
        EditorGUI.PropertyField(line1[1], property.FindPropertyRelative("offset"), GUIContent.none);
        EditorGUI.LabelField(line1[2], "   Direction");
        EditorGUI.PropertyField(line1[3], property.FindPropertyRelative("projectileDirection"), GUIContent.none);
        
        // Rotated modifiers
        EditorGUICustomUtility.ComputeRects(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight),
            line2, line2MinWidths, line2Weights);
        EditorGUI.LabelField(line2[0], "Rotated Offset ");
        EditorGUI.PropertyField(line2[1], property.FindPropertyRelative("rotatedOffset"), GUIContent.none);
        EditorGUI.LabelField(line2[2], "   Rotation");
        EditorGUI.PropertyField(line2[3], property.FindPropertyRelative("rotation"), GUIContent.none);

        // Line 3
        EditorGUICustomUtility.ComputeRects(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight*2 + 4, position.width, EditorGUIUtility.singleLineHeight),
            line3, line3MinWidths, line3Weights);
        EditorGUI.LabelField(line3[0], "Add rotation to direction");
        EditorGUI.PropertyField(line3[1], property.FindPropertyRelative("rotationAffectsProjectile"), GUIContent.none);
        EditorGUI.LabelField(line3[2], "   Speed");
        var speedMode = property.FindPropertyRelative("speedMode");
        EditorGUI.PropertyField(line3[3], speedMode, GUIContent.none);
        EditorGUI.BeginDisabledGroup(speedMode.intValue == (int)ProjectileModifiers.SpeedApplyMode.Unused);
        EditorGUI.PropertyField(line3[4], property.FindPropertyRelative("speed"), GUIContent.none);
        EditorGUI.EndDisabledGroup();
        EditorGUI.LabelField(line3[5], "   invert");
        EditorGUI.PropertyField(line3[6], property.FindPropertyRelative("invert"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3 + 4;
    }
}
