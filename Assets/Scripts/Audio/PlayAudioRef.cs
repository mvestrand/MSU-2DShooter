using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioRef : MonoBehaviour
{
    [SerializeField] AudioRef audioRef;
    [SerializeField] AudioSource source;
    [SerializeField] float volume = 1f;
    [SerializeField] bool playOnAwake = true;
    [SerializeField] float delay = 0;

    // void Update() {
    //     source.
    // }

    public IEnumerator Play() {
        if (delay > 0) {
            yield return new WaitForSeconds(delay);
        }
        audioRef?.PlayAudio(source, volume);
    }

    void OnValidate() {
        if (source == null) {
            source = GetComponent<AudioSource>();
        }
    }


    void OnEnable() {
        if (playOnAwake) {
            StartCoroutine(Play());
        }
    }

}
