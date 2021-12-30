using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;


namespace MVest {

[InlineProperty][System.Serializable]
public struct EffectRef {
    [HideLabel][HorizontalGroup(0.5f)]
    public Effect effect;
    [Tooltip("Offset to spawn the effect at (rotated by the given rotation)")]
    [HorizontalGroup(0.5f)]
    [LabelWidth(40)]
    public Vector3 offset;
    public Effect Fire() {
        return effect?.Fire();
    }

    public Effect Fire(Vector3 pos, Quaternion rot) {
        return effect?.Fire(pos + rot * offset, rot);
    }

    public Effect Fire(Transform transform) {
        return effect?.Fire(transform.TransformPoint(offset), transform.rotation);
    }

}
}

