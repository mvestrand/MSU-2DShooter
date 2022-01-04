using System;

using System.Collections;
using System.Collections.Generic;
using MVest;
using UnityEngine;

using Sirenix.OdinInspector;

public sealed class NamedEffects : EffectList, ISerializationCallbackReceiver {

    [SerializeField][LabelText("Effects")] 
    private Dictionary<string, EffectRef> _effectsList = new Dictionary<string, EffectRef>();

    protected override void GetEffects(out EffectRef[] effects) {
        effects = new EffectRef[_effectsList.Count];
        _effectsList.Values.CopyTo(effects, 0);
    }

    protected override void OnAfterDeserialize() {
        base.OnAfterDeserialize();
        _effects = null;
    }

    public void Play(string effectName) {
        if (_effectsList.TryGetValue(effectName, out var effect))
            effect.Play();
        else
            Debug.LogWarningFormat("Sound effect \"{0}\" not found in named effects list ({1})", effectName, this.name);
        
    }
    public void Play(string effectName, Vector3 pos, Quaternion rot) {
        if (_effectsList.TryGetValue(effectName, out var effect))
            effect.Play(pos, rot);
        else
            Debug.LogWarningFormat("Sound effect \"{0}\" not found in named effects list ({1})", effectName, this.name);
    }

    public void Play(string effectName, Transform transform) {
        if (_effectsList.TryGetValue(effectName, out var effect))
            effect.Play(transform);
        else
            Debug.LogWarningFormat("Sound effect \"{0}\" not found in named effects list ({1})", effectName, this.name);
    }
}
