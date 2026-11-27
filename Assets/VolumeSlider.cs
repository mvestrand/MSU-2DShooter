using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {

    public AudioMixer mixer;
    public string mixerGroupName;
    public Slider slider;
    public int scale = 100;
    public int shift = -20;

    private float minValue = -80;


/*     void Start() {
        if (slider == null) {
            Debug.LogWarning("Slider reference not set");
        } else if (mixer == null) {
            Debug.LogWarning("Mixer reference not set");
        } else if (mixer.GetFloat(mixerGroupName, out float cur_level)) {
            var level_percent = Mathf.Pow(10, (cur_level - shift) / 20);
            slider.value = Mathf.RoundToInt(level_percent*scale);
        } else {
            Debug.LogWarning("Unrecognized mixer group name: \"" + mixerGroupName + "\"");
        }        
    }
 */
    public void SetValue() {
        if (slider.value > 0) {
            mixer.SetFloat(mixerGroupName, Mathf.Log10(slider.value/scale) * 20 + shift);
        } else {
            mixer.SetFloat(mixerGroupName, minValue);
        }
    }

}
