using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

using MVest;


public abstract class EffectList : SerializedScriptableObject {

    protected EffectRef[] _effects;
    public EffectRef[] Effects {
        get {
            if (_effects == null) {
                this.GetEffects(out _effects);
            }
            return _effects;
        }
    }
    
    protected abstract void GetEffects(out EffectRef[] effects);

    public void RequestPreallocate() {
        foreach (var effect in Effects) {
            effect.RequestPreallocate();
        }
    }

    public void CancelPreallocate() {
        foreach (var effect in Effects) {
            effect.CancelPreallocate();
        }
    }
}

public struct EffectListRef<TDerived> where TDerived : EffectList {

    [HideLabel][SerializeField] TDerived effects;
    public static implicit operator TDerived(EffectListRef<TDerived> l) => l.effects;
}
