using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor {

    private const int lineSteps = 10;
    private const float velocityScale = 0.5f;
    private const float accelerationScale = 0.05f;
    private const bool normalizeVelocity = true;
    private const bool showVelocity = true;
    private const bool showAcceleration = true;
    private const float curveWidth = 2f;


    private static readonly Color curveColor = Color.white;
    private static readonly Color lineColor = Color.gray;
    private static readonly Color velocityColor = Color.green;
    private static readonly Color accelerationColor = Color.blue;

    private BezierCurve curve;
    private Transform handlesTransform;
    private Quaternion handlesRotation;


    private void OnSceneGUI () {
        curve = target as BezierCurve;
        handlesTransform = curve.transform;
        handlesRotation = (Tools.pivotRotation == PivotRotation.Local ? handlesTransform.rotation : Quaternion.identity);

        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowPoint(1);
        Vector3 p2 = ShowPoint(2);
        Vector3 p3 = ShowPoint(3);

        Handles.color = lineColor;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);

        if (showVelocity) ShowVelocity();
        if (showAcceleration) ShowAcceleration();

        Handles.DrawBezier(p0, p3, p1, p2, curveColor, null, curveWidth);
    }

    private void ShowVelocity () {
        Handles.color = velocityColor;
        Vector3 point = curve.GetPoint(0);
        for (int i = 0; i <= lineSteps; i++) {
            float t = i / (float)lineSteps;
            point = curve.GetPoint(t);
            Handles.DrawLine(point, point + (normalizeVelocity ? curve.GetDirection(t) : curve.GetVelocity(t)) * velocityScale);
        }
    }

    private void ShowAcceleration () {
        Handles.color = accelerationColor;
        Vector3 point = curve.GetPoint(0);
        for (int i = 0; i <= lineSteps; i++) {
            float t = i / (float)lineSteps;
            point = curve.GetPoint(t);
            Handles.DrawLine(point, point + curve.GetAcceleration(t) * accelerationScale);
        }
    }


    private Vector3 ShowPoint(int index) {
        Vector3 point = handlesTransform.TransformPoint(curve.points[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handlesRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(curve, "Move Point");
            EditorUtility.SetDirty(curve);
            curve.points[index] = handlesTransform.InverseTransformPoint(point);
        }
        return point;
    }
}
