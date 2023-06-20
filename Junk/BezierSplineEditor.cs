using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineEditor : Editor {

    private const int curveSteps = 10;
    private const float velocityScale = 0.5f;
    private const float accelerationScale = 0.05f;
    private const bool normalizeVelocity = true;
    private const bool showVelocity = true;
    private const bool showAcceleration = true;
    private const float curveWidth = 2f;
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;
    private int selectedIndex = -1;
	private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

    private static readonly Color pointColor = Color.white;
    private static readonly Color curveColor = Color.white;
    private static readonly Color lineColor = Color.gray;
    private static readonly Color velocityColor = Color.green;
    private static readonly Color accelerationColor = Color.blue;

    private BezierSpline spline;
    private Transform handlesTransform;
    private Quaternion handlesRotation;

    public override void OnInspectorGUI() {
        spline = target as BezierSpline;
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount) {
            DrawSelectedPointInspector();
        }
        if (GUILayout.Button("Add Curve")) {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }

	private void DrawSelectedPointInspector() {
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetControlPoint(selectedIndex, point);
		}
		EditorGUI.BeginChangeCheck();
		BezierControlPointMode mode = (BezierControlPointMode)
			EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Change Point Mode");
			spline.SetControlPointMode(selectedIndex, mode);
			EditorUtility.SetDirty(spline);
		}	
    }

    private void OnSceneGUI () {
        spline = target as BezierSpline;
        handlesTransform = spline.transform;
        handlesRotation = (Tools.pivotRotation == PivotRotation.Local ? handlesTransform.rotation : Quaternion.identity);

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.ControlPointCount; i += 3) {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i+1);
            Vector3 p3 = ShowPoint(i+2);

            Handles.color = lineColor;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, curveColor, null, curveWidth);
            p0 = p3;
        }

        if (showVelocity) ShowVelocity();
        if (showAcceleration) ShowAcceleration();

    }

    private void ShowVelocity () {
        Handles.color = velocityColor;
        Vector3 point = spline.GetPoint(0);
        int steps = curveSteps * spline.CurveCount;
        for (int i = 0; i <= steps; i++) {
            float t = i / (float)steps;
            point = spline.GetPoint(t);
            Handles.DrawLine(point, point + (normalizeVelocity ? spline.GetDirection(t) : spline.GetVelocity(t)) * velocityScale);
        }
    }

    private void ShowAcceleration () {
        Handles.color = accelerationColor;
        Vector3 point = spline.GetPoint(0);
        int steps = curveSteps * spline.CurveCount;
        for (int i = 0; i <= steps; i++) {
            float t = i / (float)steps;
            point = spline.GetPoint(t);
            Handles.DrawLine(point, point + spline.GetAcceleration(t) * accelerationScale);
        }
    }


    private Vector3 ShowPoint(int index) {
        Vector3 point = handlesTransform.TransformPoint(spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
        if (Handles.Button(point, handlesRotation, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
            selectedIndex = index;
            Repaint();
        }
        if (selectedIndex == index) {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handlesRotation);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(index, handlesTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }
}
