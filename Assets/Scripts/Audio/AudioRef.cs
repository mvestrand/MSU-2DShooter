using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName="SoundRef")]
public class AudioRef : ScriptableObject {

    [SerializeField] private AudioClip clip;
    [SerializeField] private float minInterval = 0.015f;

    private float lastPlayed = float.NegativeInfinity;

    private void OnEnable() {
        lastPlayed = float.NegativeInfinity;
    }

    public void PlayAudio(AudioSource source, float volume) {
        if (Time.time >= lastPlayed + minInterval) {
            lastPlayed = Time.time;
            source.PlayOneShot(clip, volume);
        }
    }

}
