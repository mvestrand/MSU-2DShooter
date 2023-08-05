using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct FloatTransform {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public void SetLocal(Transform tf) {
        position = tf.localPosition;
        rotation = tf.localRotation;
        scale = tf.localScale;
    }

    public void ApplyLocal(Transform tf) {
        tf.localPosition = position;
        tf.localRotation = rotation;
        tf.localScale = scale;
    }

    public static void InterpolateLocal(Transform target, in FloatTransform a, in FloatTransform b, float t) {
        target.localPosition = Vector3.Lerp(a.position, b.position, t);
        target.localRotation = Quaternion.Slerp(a.rotation, b.rotation, t);
        target.localScale = Vector3.Lerp(a.scale, b.scale, t);
    }
}
