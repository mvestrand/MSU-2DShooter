using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[ExecuteAlways]
public class AudioMixerParameterControl : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] string parameterName;
    public float value;
    [SerializeField] private float disabledValue;

    void OnEnable() {
        mixer.SetFloat(parameterName, value);
    }

    void OnDisable() {
        mixer.SetFloat(parameterName, disabledValue);

    }

    // Update is called once per frame
    void Update() {
        mixer.SetFloat(parameterName, value);
    }
}
