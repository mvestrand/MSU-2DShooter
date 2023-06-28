using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FloatCurveMode : int {
    Constant,
    Curve,
    Disabled,
    CurvePlusConstant
}

[System.Serializable]
public class FloatCurve {
    public FloatCurveMode mode;
    public float value;
    public float curveScale;
    public AnimationCurve curve;

    public FloatCurve() {
        mode = FloatCurveMode.Constant;
        value = 0;
        curve = new AnimationCurve();
    }

    public bool Enabled {
        get { return mode != FloatCurveMode.Disabled; }
    }

    public bool UsesCurve {
        get { return mode == FloatCurveMode.Curve || mode == FloatCurveMode.CurvePlusConstant;  }
    }

    public float Get(float t, float defaultValue) {
        return mode switch
        {            
            FloatCurveMode.Constant => value,
            FloatCurveMode.Curve => (curve != null ? curveScale * curve.Evaluate(t) : defaultValue),
            FloatCurveMode.CurvePlusConstant => (curve != null ? curveScale * curve.Evaluate(t) + value : value),
            _ => defaultValue
        };
    }
}
