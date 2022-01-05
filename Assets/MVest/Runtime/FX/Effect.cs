using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MVest {

[SelectionBase]
public class Effect : PooledMonoBehaviour
{

    [Tooltip("The time before the effect is removed (sec)")]
    [SerializeField] private float lifetime = 1;
    private AudioSource _audio;
    private Animator _animator;

    private ParticleSystem _particles;

    private WaitForSeconds waitForSeconds;

    protected void Awake() {
        waitForSeconds = new WaitForSeconds(lifetime);
    }

    public override void Restart()
    {
        //_audio?.Play();
        PlayAudioBackwardsFix(false);
        //_animator?.Play("Entry", -1, 0); // ??????
        _particles?.Clear();
        _particles?.Play();
        if (lifetime >= 0)
            StartCoroutine(ReleaseOnLifetimeEnd());
    }

    // Start is called before the first frame update
    protected void Start()
    {
        PlayAudioBackwardsFix(true);
    }

    IEnumerator ReleaseOnLifetimeEnd()
    {
        yield return waitForSeconds;
        this?.Release(); // The object may have been destroyed early
    }

    public Effect Play()
    {
        return this.Get<Effect>();
    }

    public Effect Play(Vector3 pos, Quaternion rot)
    {
        return this.Get<Effect>(pos, rot);
    }

    public Effect Play(Vector3 pos, Quaternion rot, Transform parent) {
        return this.Get<Effect>(pos, rot, parent);
    }

    // [ContextMenu("Play Audio")]
    // public void TestAudio() {
    //     AudioSource temp = _audio;
    //     if (_audio == null) {
    //         _audio = GetComponent<AudioSource>();
    //         if (_audio == null) {
    //             Debug.LogWarning("The audio source component could not be found.");
    //         }
    //     }
    //     PlayAudioBackwardsFix(false);
    //     _audio = temp;
    // }

    private void PlayAudioBackwardsFix(bool onlyOnAwake) {
        if (_audio != null) {
            if (_audio.pitch > 0) {
                if (!onlyOnAwake)
                    _audio.Play();
            } else if (_audio.playOnAwake || !onlyOnAwake) {
                _audio.Play();
                _audio.time = _audio.clip.length - 0.001f;
                // _audio.Play();
            }
        }
    }

    private void GetComponents() {
        if (_audio == null)
            _audio = GetComponent<AudioSource>();
        if (_animator == null)
            _animator = GetComponent<Animator>();
        if (_particles == null) 
            _particles = GetComponent<ParticleSystem>();
    }

    private void PlayThis() {

    }

    [ContextMenu("Play audio")]
    public void PlayInEditor() {
        GetComponents();
        PlayAudioBackwardsFix(false);
    }




}

}
