using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

public class Effect : PooledMonoBehaviour {

    [SerializeField] private float lifetime = 1;
    private AudioSource _audio;
    private Animator _animator;
    private ParticleSystem _particles;

    protected override void Reset() {
        _audio?.Play();
        _animator?.Play("Entry", -1, 0); // ??????
        _particles?.Clear();
        _particles?.Play();

    }

    // Start is called before the first frame update
    void Start() {
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _particles = GetComponent<ParticleSystem>();
    }

    



}
