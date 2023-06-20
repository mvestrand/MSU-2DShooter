using UnityEngine;

public static class CurveMath {
    
    public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        t = Mathf.Clamp01(t);
        float s = 1f - t;
        return s*s * p0  
            +  2f*s*t * p1  
            +  t*t * p2;
    }

    public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        t = Mathf.Clamp01(t);
        float s = 1f - t;
        return s*s*s * p0 
             + 3f*s*s*t * p1 
             + 3f*s*t*t * p2 
             + t*t*t * p3;
    }

    public static Vector3 BezierVelocity(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        t = Mathf.Clamp01(t);
        return 2f * (1 - t) * (p1 - p0) 
             + 2f * t * (p2 - p1);
    }


    public static Vector3 BezierVelocity(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        t = Mathf.Clamp01(t);
        float s = 1f - t;
        return 3f*s*s * (p1 - p0) 
             + 6f*s*t * (p2 - p1) 
             + 3f*t*t * (p3 - p2);
    }
    
    public static Vector3 BezierAcceleration(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        return 2f * (p0 + p2 - 2f * p1);
    }

    public static Vector3 BezierAcceleration(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        t = Mathf.Clamp01(t);
        return 6f * (  (t - 1) * (p1 - p0)
                     + (1 - 2*t) * (p2 - p1)
                     + t * (p3 - p2));
    }

}
