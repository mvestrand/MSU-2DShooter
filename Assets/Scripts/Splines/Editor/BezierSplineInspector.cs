using UnityEditor;
using UnityEngine;

using MVest.Unity;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor {

	private const int stepsPerCurve = 10;
	private const float directionScale = 0.5f;
	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

    private static readonly Color slowColor = Color.green;
    private static readonly Color fastColor = Color.red;



    private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

	private BezierSpline spline;
	//private FloatTransform handleTransform;
	private Transform handleTransform;
	private Quaternion handleRotation;
	private int selectedIndex = -1;

	public override void OnInspectorGUI () {
        //base.OnInspectorGUI();
        spline = target as BezierSpline;
		EditorGUI.BeginChangeCheck();
		bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Toggle Loop");
			EditorUtility.SetDirty(spline);
			spline.Loop = loop;
		}
        bool pointIsSelected = selectedIndex >= 0 && selectedIndex < spline.ControlPointCount;

        EditorGUI.BeginDisabledGroup(!pointIsSelected);

        DrawSelectedPointInspector(pointIsSelected);

		if (GUILayout.Button("Add Waypoint")) {
			Undo.RecordObject(spline, "Add Waypoint");
			if (selectedIndex >= spline.ControlPointCount-2) {
                spline.AddCurve();
            } else {
				spline.InsertWaypoint(selectedIndex / 3);
			}
			EditorUtility.SetDirty(spline);
		}
		if (GUILayout.Button("Remove Waypoint")) {
			Undo.RecordObject(spline, "Remove Waypoint");
			spline.RemoveWaypoint((selectedIndex+1) / 3);
			EditorUtility.SetDirty(spline);
		}
        EditorGUI.EndDisabledGroup();

		if (GUILayout.Button("Bake")) {
            var renderer = UpdateLineRenderer();
			if (renderer != null)
                EditorUtility.SetDirty(renderer);
            spline.UpdateArcLengths();
            EditorUtility.SetDirty(spline);
        }
    }

	private void DrawSelectedPointInspector(bool pointIsSelected) {
        Vector3 positionValue = (pointIsSelected ? spline.GetControlPoint(selectedIndex) : Vector3.zero);
        BezierControlPointMode modeValue = (pointIsSelected ? spline.GetControlPointMode(selectedIndex) : BezierControlPointMode.Free);

        GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", positionValue);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetControlPoint(selectedIndex, point);
		}
		EditorGUI.BeginChangeCheck();
		BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", modeValue);
		if (EditorGUI.EndChangeCheck()) {
			
			Undo.RecordObject(spline, "Change Point Mode");
			spline.SetControlPointMode(selectedIndex, mode);
			EditorUtility.SetDirty(spline);
		}
	}

	private void OnSceneGUI () {
		spline = target as BezierSpline;
		handleTransform = spline.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ?
			handleTransform.rotation : Quaternion.identity;
		
		Vector3 p0 = ShowPoint(0);
		for (int i = 1; i < spline.ControlPointCount; i += 3) {
			Vector3 p1 = ShowPoint(i);
			Vector3 p2 = ShowPoint(i + 1);
			Vector3 p3 = ShowPoint(i + 2);
			
			Handles.color = Color.gray;
			Handles.DrawLine(p0, p1);
			Handles.DrawLine(p2, p3);
			
			Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
			p0 = p3;
		}
		//ShowDirections();
        //ShowVelocityColor();
    }

	private void ShowVelocityColor () {
		Vector3 point = spline.GetPoint(0f);
		int steps = stepsPerCurve * spline.CurveCount;
        float maxDist = float.NegativeInfinity;
        float minDist = float.PositiveInfinity;
        for (int i = 1; i <= steps; i++) {
			var nextPoint = spline.GetPoint(i / (float)steps);
            float dist = Vector3.Magnitude(nextPoint - point);
            maxDist = Mathf.Max(maxDist, dist);
            minDist = Mathf.Min(minDist, dist);
            point = nextPoint;
        }

        point = spline.GetPoint(0f);
        for (int i = 1; i <= steps; i++) {
			var nextPoint = spline.GetPoint(i / (float)steps);
            float dist = Vector3.Magnitude(nextPoint - point);
            float t = (dist - minDist) / (maxDist - minDist);
			
            var colorVec = Vector3.Slerp(new Vector3(slowColor.r, slowColor.g, slowColor.b), new Vector3(fastColor.r, fastColor.g, fastColor.b), t);
            Color finalColor = new Color(colorVec.x, colorVec.y, colorVec.z, Mathf.Lerp(slowColor.a, fastColor.a, t));
            Handles.color = finalColor;
            Handles.DrawLine(point, nextPoint, 5f);
            point = nextPoint;
        }

	}

	private void ShowDirections () {
		Handles.color = Color.green;
		Vector3 point = spline.GetPoint(0f);
		Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);

		int steps = stepsPerCurve * spline.CurveCount;
		for (int i = 1; i <= steps; i++) {
			point = spline.GetPoint(i / (float)steps);
			Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
		}
	}

	private Vector3 ShowPoint (int index) {
		Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
		float size = HandleUtility.GetHandleSize(point);
		if (index == 0) {
			size *= 2f;
		}
		Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
		if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
			selectedIndex = index;
			Repaint();
		}
		if (selectedIndex == index) {
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
			}
		}
		return point;
	}

	LineRenderer UpdateLineRenderer() {
        var spline = target as BezierSpline;
        LineRenderer renderer = spline.GetComponent<LineRenderer>();
		if (renderer == null)
            renderer = InitLineRenderer();

		int steps = stepsPerCurve * spline.CurveCount;

        Vector3[] points = new Vector3[steps+1];
        for (int i = 0; i <= steps; i++) {
			points[i] = spline.GetPoint(i / (float)steps, worldSpace:false);
        }

        renderer.positionCount = steps+1;
        renderer.SetPositions(points);

        return renderer;
    }

	LineRenderer InitLineRenderer() {
        var gameObject = (target as BezierSpline).gameObject;
        var renderer = gameObject.AddComponent<LineRenderer>();
        renderer.useWorldSpace = false;
        renderer.startColor = Color.gray;
        renderer.endColor = Color.gray;
        renderer.widthMultiplier = 0.1f;
        renderer.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Line.mat");
        return renderer;
    }

}

