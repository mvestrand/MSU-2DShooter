using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct OptionalFloat {
    public bool enabled;
    public float value;

    public float Get(float defaultValue) {
        return (enabled ? value : defaultValue);
    }
}
