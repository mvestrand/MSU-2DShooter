using System.Collections;
using System.Collections.Generic;
using MVest.Unity;
using UnityEngine;
using UnityEngine.Audio;

[ExecuteAlways]
public class AudioMixerParameterControl : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] string parameterName;
    public float min;
    public float max;
    [Range(0,1)]public float value;
    public bool logScale;
    [SerializeField] private float disabledValue;

    void OnEnable() {
        mixer.SetFloat(parameterName, GetValue());
    }

    void OnDisable() {

        mixer.SetFloat(parameterName, disabledValue);

    }

    // Update is called once per frame
    void Update() {
        mixer.SetFloat(parameterName, GetValue());
    }

    float GetValue() {
        if (logScale) {
            return min * Mathf.Pow(max / min, value);
        }
        return Mathf.Lerp(min, max, value);
    }
}
