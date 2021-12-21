using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

public class Effect : PooledMonoBehaviour {

    [Tooltip("The time before the effect is removed (sec)")]
    [SerializeField] private float lifetime = 1;
    private AudioSource _audio;
    private Animator _animator;
    
    private ParticleSystem _particles;

    protected override void Restart() {
        _audio?.Play();
        _animator?.Play("Entry", -1, 0); // ??????
        _particles?.Clear();
        _particles?.Play();
        if (lifetime >= 0)
            StartCoroutine(ReleaseOnLifetimeEnd());
    }

    // Start is called before the first frame update
    void Start() {
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _particles = GetComponent<ParticleSystem>();
    }

    IEnumerator ReleaseOnLifetimeEnd() {
        yield return new WaitForSeconds(lifetime);
        this.Release();
    }

    public Effect Fire() {
        return this.Get().GetComponent<Effect>();
    }





}
