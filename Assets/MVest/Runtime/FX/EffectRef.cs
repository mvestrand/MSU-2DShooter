using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;


namespace MVest {

[InlineProperty][System.Serializable]
public struct EffectRef {
    [HideLabel][HorizontalGroup(0.7f)]
    public Effect effect;
    [HorizontalGroup(0.3f)]
    [LabelWidth(50)]
    [Min(0)][Tooltip("How many of copies of the effect to preload for this reference")]
    [SerializeField] private int preload;
    [Tooltip("Offset to spawn the effect at (rotated by the given rotation)")]
    //[HorizontalGroup(0.3f)]
    [LabelWidth(40)]
    public Vector3 offset;
    public Effect Play() {
        return effect?.Play();
    }

    public Effect Play(Vector3 pos, Quaternion rot) {
        return effect?.Play(pos + rot * offset, rot);
    }

    public Effect Play(Transform transform) {
        return effect?.Play(transform.TransformPoint(offset), transform.rotation);
    }

    public void RequestPreallocate() {
        effect?.RequestPreallocate(preload);
    }

    public void CancelPreallocate() {
        effect?.CancelPreallocate(preload);
    }
}


}

