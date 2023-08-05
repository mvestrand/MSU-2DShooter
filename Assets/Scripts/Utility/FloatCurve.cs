using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FloatCurveMode : int {
    Constant,
    Curve,
    Disabled,
    CurvePlusConstant,
    BetweenTwoConstants
}

[System.Serializable]
public class FloatCurve {
    public FloatCurveMode mode;
    public float value;
    public float curveScale;
    public float min;
    public float max;
    public AnimationCurve curve;

    public FloatCurve() {
        mode = FloatCurveMode.Constant;
        value = 0;
        curve = new AnimationCurve();
    }
    public FloatCurve(FloatCurveMode mode) {
        this.mode = mode;
        value = 0;
        curve = new AnimationCurve();
    }

    public bool Enabled {
        get { return mode != FloatCurveMode.Disabled; }
    }

    public bool UsesCurve {
        get { return mode == FloatCurveMode.Curve || mode == FloatCurveMode.CurvePlusConstant;  }
    }

    public bool UsesT {
        get { return mode == FloatCurveMode.Curve || mode == FloatCurveMode.CurvePlusConstant || mode == FloatCurveMode.BetweenTwoConstants;  }
    }

    public float Get(float t, float defaultValue) {
        return mode switch
        {            
            FloatCurveMode.Constant => value,
            FloatCurveMode.Curve => (curve != null ? curveScale * curve.Evaluate(t) : defaultValue),
            FloatCurveMode.CurvePlusConstant => (curve != null ? curveScale * curve.Evaluate(t) + value : value),
            FloatCurveMode.BetweenTwoConstants => (1-t)*min + t*max,
            _ => defaultValue
        };
    }
}
