using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;


using Sirenix.OdinInspector;

public class TimedSequence : MonoBehaviour, ISequence
{
    #region Configuration Fields

    [Tooltip("Play this sequence immediately")]
    [SerializeField] private bool playOnAwake = false;

    [FoldoutGroup("Time Settings")]
    [HorizontalGroup("Time Settings/MinBlockTime", Width = 15)]
    [HideLabel]
    [Tooltip("Whether or not this sequence has a minimum block time")]
    [SerializeField] private bool useMinBlockTime = false;

    [FoldoutGroup("Time Settings")]
    [HorizontalGroup("Time Settings/MinBlockTime")][EnableIf("useMinBlockTime")]
    [Tooltip("Minimum time the sequence will block for")]
    [SerializeField] private float minBlockTime = 1f;


    [FoldoutGroup("Time Settings")]
    [HorizontalGroup("Time Settings/MaxBlockTime", Width = 15)]
    [HideLabel]
    [Tooltip("Whether or not this sequence has a maximum block time")]
    [SerializeField] private bool useMaxBlockTime = false;

    [FoldoutGroup("Time Settings")]
    [HorizontalGroup("Time Settings/MaxBlockTime")][EnableIf("useMaxBlockTime")]
    [Tooltip("Maximum time the sequence will block for")]
    [SerializeField] private float maxBlockTime = 10f;


    [FoldoutGroup("Time Settings")]
    [HorizontalGroup("Time Settings/SoftTimeLimit", Width = 15)]
    [HideLabel]
    [Tooltip("Whether or not to tell objects to end after a given time")]
    [SerializeField] private bool useSoftTimeLimit = false;

    [FoldoutGroup("Time Settings")]
    [HorizontalGroup("Time Settings/SoftTimeLimit")][EnableIf("useSoftTimeLimit")]
    [Tooltip("Maximum time before telling objects to end")]
    [SerializeField] private float softTimeLimit = 10f;


    [FoldoutGroup("Time Settings")]
    [HorizontalGroup("Time Settings/HardTimeLimit", Width = 15)]
    [HideLabel]
    [Tooltip("Whether or not to force objects to end after a given time")]
    [SerializeField] private bool useHardTimeLimit = false;

    [FoldoutGroup("Time Settings")]
    [HorizontalGroup("Time Settings/HardTimeLimit")][EnableIf("useHardTimeLimit")]
    [Tooltip("Maximum time before forcing objects to end")]
    [SerializeField] private float hardTimeLimit = 20f;

    [FoldoutGroup("Events")]
    public UnityEvent onPlay;
    [FoldoutGroup("Events")]
    public UnityEvent onFinish;
    [FoldoutGroup("Events")]
    public UnityEvent onCleanup;

    [Tooltip("A PlayableDirector component to play")]
    [SerializeField] private PlayableDirector director;
    [Tooltip("An animator component to play")]
    [SerializeField] private Animator animator;
    #endregion


    private bool _waitingOnMinTime;

    private float _startTime = Mathf.NegativeInfinity;
    public float StartTime {get { return _startTime; } }

    private SequenceState _state = SequenceState.Unplayed;
    public SequenceState State { get { return _state; } }

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
        this.enabled = true;
        if (director != null)
            director.Play();
        onPlay.Invoke();
    }

    public virtual void Finish()
    {
        _state = SequenceState.Finishing;
        if (animator != null)
            animator.SetTrigger("finish");
        onFinish.Invoke();

    }

    public virtual void Cleanup()
    {
        _state = SequenceState.CleanedUp;
        if (director != null)
            director.Stop();
        this.gameObject.SetActive(false);
        onCleanup.Invoke();
    }

    public virtual void Clear()
    {
        // Clean up if called while the sequence is running
        if (_state == SequenceState.Playing || _state == SequenceState.Finishing) {
            bool wasActive = this.gameObject.activeInHierarchy;
            Cleanup();
            if (wasActive)
                this.gameObject.SetActive(true);
        }
        _state = SequenceState.Unplayed;
        _startTime = Mathf.NegativeInfinity;
    }

    public virtual void AllowUnblock() {
        _forceBlock = false;
    }

    public virtual void AllowFinish() {
        _allowFinish = true;
    }

    public virtual void AllowCleanup() {
        _allowCleanup = true;
    }

    

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Check state to prevent duplicate plays calls when play is called externally
        if (playOnAwake && _state == SequenceState.Unplayed) { 
            this.Play();
        }
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

    private bool PastTime(float timelimit) {
        return Time.time > _startTime + timelimit;
    }

}
