using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ReversableAudioSource : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] private bool useMaxTime;
    [SerializeField] private float _setMaxTime;
    [SerializeField] private bool useMinTime;
    [SerializeField] private float _setMinTime;

    [SerializeField] private float maxTime;
    [SerializeField] private float minTime;
    [SerializeField] private float currentTime;
    [SerializeField] private float defaultSpeed = 1;
    private float _currSpeed = 1;
    public float Speed {
        get { return _currSpeed; }
        set { SetSpeed(value); }
    }



    public void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        audioSource.time = 1;
        audioSource.pitch = _currSpeed = defaultSpeed;
    }

    void Update() {
        currentTime = audioSource.time;
        if (audioSource.isPlaying) {
        }
    }

    public void SetSpeed(float speed) {
    }

    public void Forward() {
        if (_currSpeed < 0)
            SetSpeed(-_currSpeed);
    }



    public void Reverse() {
        if (_currSpeed > 0)
            SetSpeed(-_currSpeed);
    }
}
