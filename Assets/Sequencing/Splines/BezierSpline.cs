﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class BezierSpline : MonoBehaviour {

    [SerializeField]
	private List<Vector3> points;

	[SerializeField]
	private List<BezierControlPointMode> modes;

	[SerializeField]
	private bool loop;

	public bool Loop {
		get {
			return loop;
		}
		set {
			loop = value;
			if (value == true) {
				modes[modes.Count - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount {
		get {
			return points.Count;
		}
	}

	public Vector3 GetControlPoint (int index) {
		return points[index];
	}

	public void SetControlPoint (int index, Vector3 point) {
		if (index % 3 == 0) {
			Vector3 delta = point - points[index];
			if (loop) {
				if (index == 0) {
					points[1] += delta;
					points[points.Count - 2] += delta;
					points[points.Count - 1] = point;
				}
				else if (index == points.Count - 1) {
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else {
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else {
				if (index > 0) {
					points[index - 1] += delta;
				}
				if (index + 1 < points.Count) {
					points[index + 1] += delta;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
	}

	public BezierControlPointMode GetControlPointMode (int index) {
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;
		if (loop) {
			if (modeIndex == 0) {
				modes[modes.Count - 1] = mode;
			}
			else if (modeIndex == modes.Count - 1) {
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	private void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Count - 1)) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0) {
				fixedIndex = points.Count - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Count) {
				enforcedIndex = 1;
			}
		}
		else {
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Count) {
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Count - 2;
			}
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
		}
		points[enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount {
		get {
			return (points.Count - 1) / 3;
		}
	}

	public Vector3 GetPoint (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Count - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}
	
	public Vector3 GetVelocity (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Count - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
	}
	
	public Vector3 GetCurvePoint(float t, int i) {
        t = Mathf.Clamp01(t);
        i = i * 3;
        return Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t);
    }
	public Vector3 GetCurveVelocity(float t, int i) {
        t = Mathf.Clamp01(t);
        i = i * 3;
        return Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t);
    }

	public Vector3 GetDirection (float t) {
		return GetVelocity(t).normalized;
	}

	public void InsertWaypoint(int curveIndex) {
        Vector3 knot = GetCurvePoint(0.5f, curveIndex);
        Vector3 velocity = GetCurveVelocity(0.5f, curveIndex);

        //Debug.Log($"Velocity = {velocity}");
        // float distance = Vector3.Distance(points[3 * curveIndex], points[3 * curveIndex + 1])
        //     + Vector3.Distance(points[3 * curveIndex + 1], points[3 * curveIndex + 2])
        //     + Vector3.Distance(points[3 * curveIndex + 2], points[3 * curveIndex + 3]);
        // Debug.Log($"Simple distance {distance}");


        Vector3 cp1 = knot - velocity.normalized;
        Vector3 cp2 = knot + velocity.normalized;

        int i = curveIndex * 3 + 2;
        Vector3[] newPoints = { cp1, knot, cp2 };
        points.InsertRange(i, newPoints);


        modes.Insert(curveIndex + 1, modes[curveIndex]);
    }

	public void RemoveWaypoint(int waypointIndex) {
        waypointIndex = Mathf.Clamp(waypointIndex, 0, CurveCount);
        if (waypointIndex == 0 || waypointIndex == CurveCount) {
            Debug.LogWarning("Cannot delete start or end nodes of spline");
            return;
		}

		points.RemoveRange(waypointIndex * 3 - 1, 3);
        modes.RemoveAt(waypointIndex);
    }


	public void AddCurve () {
		Vector3 point = points[points.Count - 1];
		point.x += 1f;
        points.Add(point);
		point.x += 1f;
        points.Add(point);
		point.x += 1f;
        points.Add(point);

        modes.Add(modes.Last());
        //modes[modes.Count - 1] = modes[modes.Count - 1];
        EnforceMode(points.Count - 4);

		if (loop) {
			points[points.Count - 1] = points[0];
			modes[modes.Count - 1] = modes[0];
			EnforceMode(0);
		}
	}
	
	public void Reset () {
		points = new List<Vector3> {
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
		modes = new List<BezierControlPointMode> {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
	}
}