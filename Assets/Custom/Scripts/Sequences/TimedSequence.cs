using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


using Sirenix.OdinInspector;

public class TimedSequence : MonoBehaviour, ISequence
{
    #region Configuration Fields

    [Tooltip("Play this sequence immediately")]
    [SerializeField] private bool playOnAwake = false;


    [HorizontalGroup("MinBlockTime", Width = 15)]
    [HideLabel]
    [Tooltip("Whether or not this sequence has a minimum block time")]
    [SerializeField] private bool useMinBlockTime = false;

    [HorizontalGroup("MinBlockTime")][EnableIf("useMinBlockTime")]
    [Tooltip("Minimum time the sequence will block for")]
    [SerializeField] private float minBlockTime = 1f;


    [HorizontalGroup("MaxBlockTime", Width = 15)]
    [HideLabel]
    [Tooltip("Whether or not this sequence has a maximum block time")]
    [SerializeField] private bool useMaxBlockTime = false;

    [HorizontalGroup("MaxBlockTime")][EnableIf("useMaxBlockTime")]
    [Tooltip("Maximum time the sequence will block for")]
    [SerializeField] private float maxBlockTime = 10f;


    [HorizontalGroup("SoftTimeLimit", Width = 15)]
    [HideLabel]
    [Tooltip("Whether or not to tell objects to end after a given time")]
    [SerializeField] private bool useSoftTimeLimit = false;

    [HorizontalGroup("SoftTimeLimit")][EnableIf("useSoftTimeLimit")]
    [Tooltip("Maximum time before telling objects to end")]
    [SerializeField] private float softTimeLimit = 10f;


    [HorizontalGroup("HardTimeLimit", Width = 15)]
    [HideLabel]
    [Tooltip("Whether or not to force objects to end after a given time")]
    [SerializeField] private bool useHardTimeLimit = false;

    [HorizontalGroup("HardTimeLimit")][EnableIf("useHardTimeLimit")]
    [Tooltip("Maximum time before forcing objects to end")]
    [SerializeField] private float hardTimeLimit = 20f;
    #endregion


    private bool _waitingOnMinTime;

    private float _startTime = Mathf.NegativeInfinity;
    public float StartTime {get { return _startTime; } }

    private SequenceState _state = SequenceState.Unplayed;
    public SequenceState State { get { return _state; } }
    [SerializeField] private PlayableDirector director;
    [SerializeField] private Animator animator;

    private bool _forceBlock = true;
    private bool _allowFinish = false;
    private bool _allowCleanup = false;

    
    public bool IsRunning() {
        return _state == SequenceState.Playing || _state == SequenceState.Finishing;
    }

    public bool Block { 
        get {
            return (_state == SequenceState.Playing || _state == SequenceState.Finishing)
                && ((useMinBlockTime && !PastTime(minBlockTime))
                    || (useMaxBlockTime && !PastTime(maxBlockTime) && _forceBlock));
        }
    }


    public virtual void CheckFlag(ISequenceFlag flag, out FlagStatus status) {
        status = new FlagStatus();
    }


    public virtual void Play()
    {
        if (_state != SequenceState.Unplayed)
            this.Clear();
        _state = SequenceState.Playing;
        _forceBlock = true;
        _allowFinish = false;
        _allowCleanup = false;
        this.gameObject.SetActive(true);
        if (director != null)
            director.Play();
        if (director != null)
            animator.Play("Entry");
    }

    public virtual void Finish()
    {
        _state = SequenceState.Finishing;
        if (animator != null)
            animator.SetTrigger("finish");
    }

    public virtual void Cleanup()
    {
        Debug.Log("Cleanup()");
        _state = SequenceState.CleanedUp;
        if (director != null)
            director.Stop();
        this.gameObject.SetActive(false);
    }

    public virtual void Clear()
    {
        // Clean up if called while the sequence is running
        if (_state == SequenceState.Playing || _state == SequenceState.Finishing)
            Cleanup(); 
        _state = SequenceState.Unplayed;
        _startTime = Mathf.NegativeInfinity;
    }

    public void AllowUnblock() {
        _forceBlock = false;
    }

    public void AllowFinish() {
        Debug.Log("AllowFinish()");
        _allowFinish = true;
    }

    public void AllowCleanup() {
        Debug.Log("AllowCleanup()");
        _allowCleanup = true;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (playOnAwake) {
            this.Play();
        }
    }

    private bool PastTime(float timelimit) {
        return Time.time > _startTime + timelimit;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_state == SequenceState.Playing || _state == SequenceState.Finishing) {

            // Check if we should clean up this sequence
            if ((_allowCleanup && (PastTime(minBlockTime) || !useMinBlockTime) )
                || (useHardTimeLimit && PastTime(hardTimeLimit) ))
            {
                this.Cleanup();
            }
            // Check if we should finish this sequence
            else if (_state == SequenceState.Playing 
                && (_allowFinish || (useSoftTimeLimit && PastTime(softTimeLimit)) ))
            {
                this.Finish();
            }

        }
    }


}
