using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName="SoundRef")]
public class AudioRef : ScriptableObject {

    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private float minInterval = 0.015f;
    [SerializeField] private bool ignoreListenerPause = false;
    [SerializeField] private bool ignoreTimescale = false;

    private float lastPlayed = float.NegativeInfinity;

    private void OnEnable() {
        lastPlayed = float.NegativeInfinity;
    }

    public void PlayAudio(AudioSource source, float volume) {
        var t = (ignoreTimescale ? Time.unscaledTime : Time.time);
        if (t >= lastPlayed + minInterval) {
            lastPlayed = t;

            UpdateSourceSettings(source);
            AudioClip clip = clips[Random.Range(0, clips.Count)];
            source.PlayOneShot(clip, volume);
        }
    }

    private void UpdateSourceSettings(AudioSource source) {
        source.ignoreListenerPause = ignoreListenerPause;        
    }

}
