using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BezierControlPointMode {
    Free,
    Aligned,
    Mirrored
}

public class BezierSpline : MonoBehaviour
{
    [SerializeField]
    private Vector3[] points;
    [SerializeField]
    private BezierControlPointMode[] modes;

    public void Reset() {
        points = new Vector3[] {
            new Vector3(1,0,0),
            new Vector3(2,0,0),
            new Vector3(3,0,0),
            new Vector3(4,0,0)
        };
        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

	public BezierControlPointMode GetControlPointMode (int index) {
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		modes[(index + 1) / 3] = mode;
        EnforceMode(index);
    }
    public int CurveCount { get { return (points.Length - 1) / 3; } }
    public int ControlPointCount { get { return points.Length; } }
    public Vector3 GetControlPoint(int i) { return points[i]; }
    public void SetControlPoint(int i, Vector3 point) { points[i] = point; EnforceMode(i); }

    private void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Length - 1) {
			return;
		}
		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			enforcedIndex = middleIndex + 1;
		}
		else {
			fixedIndex = middleIndex + 1;
			enforcedIndex = middleIndex - 1;
		}	}

    private (float t, int i) GetSubcurveTime(float t) {
        if (t >= 1f) {
            return (1f, points.Length - 4);
        } else {
            t = Mathf.Clamp01(t) * CurveCount;
            int i = (int)t;
            return (t - i, i * 3);
        }
    }

    public Vector3 GetPoint(float t) {
        int i;
        (t, i) = GetSubcurveTime(t);
        return transform.TransformPoint(CurveMath.Bezier(points[i], points[i+1], points[i+2], points[i+3], t));
    }

    public Vector3 GetVelocity(float t) {
        int i;
        (t, i) = GetSubcurveTime(t);
        return transform.TransformPoint(CurveMath.BezierVelocity(points[i], points[i+1], points[i+2], points[i+3], t))
            - transform.position;
    }
    public Vector3 GetAcceleration(float t) {
        int i;
        (t, i) = GetSubcurveTime(t);
        return transform.TransformPoint(CurveMath.BezierAcceleration(points[i], points[i+1], points[i+2], points[i+3], t))
            - transform.position;
    }

    public Vector3 GetDirection(float t) {
        return GetVelocity(t).normalized;
    }

    public void AddCurve() {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;
    }
}
