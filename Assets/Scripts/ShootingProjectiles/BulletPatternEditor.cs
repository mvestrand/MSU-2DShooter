using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FireSequenceFrame))]
public class FireSequenceFrameDrawer : PropertyDrawer {

    private Rect[] line1 = new Rect[3];
    private static float[] line1MinWidths = new float[3] { 100f, 30, 100 };
    private static float[] line1Weights = new float[3] { 300, 0, 500 };
    private Rect[] line2 = new Rect[3];
    private static float[] line2MinWidths = new float[3] { 100f, 90, 20 };
    private static float[] line2Weights = new float[3] { 500, 0, 0 };
    private Rect[] line3 = new Rect[6];
    private static float[] line3MinWidths = new float[6] { 60f, 60f,60f,60f,60f,20f };
    private static float[] line3Weights = new float[6] { 0, 100, 0, 100, 0, 0 };
    private Rect[] line4 = new Rect[3];
    private static float[] line4MinWidths = new float[3] { 100f, 30, 100 };
    private static float[] line4Weights = new float[3] { 0, 0, 500 };
    private Rect[] line5 = new Rect[4];
    private static float[] line5MinWidths = new float[4] { 60f, 100, 100, 20 };
    private static float[] line5Weights = new float[4] { 0, 500, 0, 0 };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var rightAlignedNumberField = new GUIStyle(EditorStyles.numberField);
        rightAlignedNumberField.alignment = TextAnchor.LowerRight;
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUICustomUtility.ComputeRects(
			new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), 
			line1, line1MinWidths, line1Weights);

        var timeProp = property.FindPropertyRelative("time");
        float val = EditorGUI.DelayedFloatField(line1[0], GUIContent.none, timeProp.floatValue, rightAlignedNumberField);
		if (val != timeProp.floatValue) {
            timeProp.floatValue = val;
        }
        EditorGUI.LabelField(line1[1], " s   ");
        var modeProp = property.FindPropertyRelative("mode");
        EditorGUI.PropertyField(line1[2], modeProp, GUIContent.none);

		if (modeProp.intValue == (int)FireSequenceFrameMode.Projectile || modeProp.intValue == (int)FireSequenceFrameMode.Pattern) {
			EditorGUICustomUtility.ComputeRects(
				new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight+2, position.width, EditorGUIUtility.singleLineHeight), 
				line2, line2MinWidths, line2Weights);

            EditorGUI.PropertyField(line2[0], 
				property.FindPropertyRelative(
					(modeProp.intValue == (int)FireSequenceFrameMode.Pattern) ? "pattern" : "projectile"), 
				GUIContent.none);
            EditorGUI.LabelField(line2[1], "      Use spread");
            EditorGUI.PropertyField(line2[2], property.FindPropertyRelative("useControllerSpread"), GUIContent.none);

            var modsProp = property.FindPropertyRelative("modifiers");
            EditorGUI.PropertyField(
                new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight + 4, position.width, EditorGUI.GetPropertyHeight(modsProp)),
                modsProp, GUIContent.none, true);
        } else if (modeProp.intValue == (int)FireSequenceFrameMode.JumpFrame) {
			EditorGUICustomUtility.ComputeRects(
				new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight+2, position.width, EditorGUIUtility.singleLineHeight), 
				line3, line3MinWidths, line3Weights);

            EditorGUI.LabelField(line3[0], "target frame");
            EditorGUI.PropertyField(line3[1], property.FindPropertyRelative("jumpTargetFrame"), GUIContent.none);
            EditorGUI.LabelField(line3[2], "loops");
            EditorGUI.PropertyField(line3[3], property.FindPropertyRelative("maxLoops"), GUIContent.none);
            EditorGUI.LabelField(line3[4], "skip on no shoot");
            EditorGUI.PropertyField(line3[5], property.FindPropertyRelative("ignoreJumpOnNoShoot"), GUIContent.none);			
        }

		float height = EditorGUIUtility.singleLineHeight + 2 + ModeHeight(property.FindPropertyRelative("mode").intValue, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("modifiers")));

        EditorGUICustomUtility.ComputeRects(
			new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight), 
			line5, line5MinWidths, line5Weights);
        EditorGUI.LabelField(line5[0], "Effect");
        EditorGUI.PropertyField(line5[1], property.FindPropertyRelative("effect"), GUIContent.none);
        EditorGUI.LabelField(line5[2], "Should parent effect");
        EditorGUI.PropertyField(line5[3], property.FindPropertyRelative("shouldParentEffect"), GUIContent.none);

        height += EditorGUIUtility.singleLineHeight + 2;

        EditorGUICustomUtility.ComputeRects(
			new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight), 
			line4, line4MinWidths, line4Weights);
        EditorGUI.LabelField(line4[0], "Limit turn speed");
        EditorGUI.PropertyField(line4[1], property.FindPropertyRelative("shouldLimitTurnSpeed"), GUIContent.none);
        EditorGUI.PropertyField(line4[2], property.FindPropertyRelative("limitTurnSpeed"), GUIContent.none);
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3 + 6 + ModeHeight(property.FindPropertyRelative("mode").intValue, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("modifiers")));
    }

	private float ModeHeight(int mode, float modifiersHeight) {
        return mode switch
        {
            (int)FireSequenceFrameMode.NoProjectile => 0,
            (int)FireSequenceFrameMode.Projectile => EditorGUIUtility.singleLineHeight + 4 + modifiersHeight,
            (int)FireSequenceFrameMode.Pattern => EditorGUIUtility.singleLineHeight + 4 + modifiersHeight,
            (int)FireSequenceFrameMode.JumpFrame => EditorGUIUtility.singleLineHeight + 2,
            _ => 0
        };


    }
}

[CustomPropertyDrawer(typeof(ProjectileData))]
public class ProjectileDataDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // EditorGUILayout.BeginVertical();
        // EditorGUILayout.PropertyField(property.FindPropertyRelative("prefab"), GUIContent.none);
        // EditorGUILayout.PropertyField(property.FindPropertyRelative("modifiers"), GUIContent.none);
        // EditorGUILayout.EndVertical();
        EditorGUI.BeginProperty(position, label, property);

        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var modifiersProp = property.FindPropertyRelative("modifiers");


        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), 
        	property.FindPropertyRelative("prefab"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(position.x, position.y+EditorGUIUtility.singleLineHeight+2, position.width, EditorGUI.GetPropertyHeight(modifiersProp)), 
        	modifiersProp, GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + 2 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("modifiers"));
    }
}


[CustomEditor(typeof(BulletPattern))]
public class BulletPatternEditor : Editor {

    BulletPattern pattern;

    public override void OnInspectorGUI () {
        pattern = target as BulletPattern;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectiles"));

    }

}

