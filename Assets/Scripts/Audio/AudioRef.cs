using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName="SoundRef")]
public class AudioRef : ScriptableObject {

    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private float minInterval = 0.015f;

    private float lastPlayed = float.NegativeInfinity;

    private void OnEnable() {
        lastPlayed = float.NegativeInfinity;
    }

    public void PlayAudio(AudioSource source, float volume) {
        if (Time.time >= lastPlayed + minInterval) {
            lastPlayed = Time.time;
            AudioClip clip = clips[Random.Range(0, clips.Count)];
            source.PlayOneShot(clip, volume);
        }
    }

}
