using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;


using Sirenix.OdinInspector;

using MVest;

public class TimedSequence : MonoBehaviour, ISequence
{
    #region Configuration Fields

    [Tooltip("The descriptive name of this sequence")]
    [SerializeField] private string _sequenceName;


    [Tooltip("Play this sequence immediately")]
    [SerializeField] public bool playOnAwake = false;

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


    [FoldoutGroup("Debug",-10000)]
    [FoldoutGroup("Debug/TimedSequence",-10000)][ShowIf("@UnityEngine.Application.isPlaying")]
    [ShowInInspector][Sirenix.OdinInspector.ReadOnly] private float _startTime = Mathf.NegativeInfinity;
    public float StartTime {get { return _startTime; } }

    [FoldoutGroup("Debug",-10000)]
    [FoldoutGroup("Debug/TimedSequence",-10000)][ShowIf("@UnityEngine.Application.isPlaying")]
    [ShowInInspector][Sirenix.OdinInspector.ReadOnly] private SequenceState _state = SequenceState.Unplayed;
    public SequenceState State { get { return _state; } }

    [FoldoutGroup("Debug",-10000)]
    [FoldoutGroup("Debug/TimedSequence",-10000)][ShowIf("@UnityEngine.Application.isPlaying")]
    [ShowInInspector][Sirenix.OdinInspector.ReadOnly] private bool _forceBlock = true;
    [FoldoutGroup("Debug",-10000)]
    [FoldoutGroup("Debug/TimedSequence",-10000)][ShowIf("@UnityEngine.Application.isPlaying")]
    [ShowInInspector][Sirenix.OdinInspector.ReadOnly] private bool _allowFinish = false;
    [FoldoutGroup("Debug",-10000)]
    [FoldoutGroup("Debug/TimedSequence",-10000)][ShowIf("@UnityEngine.Application.isPlaying")]
    [ShowInInspector][Sirenix.OdinInspector.ReadOnly] private bool _allowCleanup = false;

    [FoldoutGroup("Debug",-10000)]
    [FoldoutGroup("Debug/TimedSequence",-10000)][ShowIf("@UnityEngine.Application.isPlaying")]
    [ShowInInspector][Sirenix.OdinInspector.ReadOnly]
    private bool _block; // For debugging

    public virtual string GetPrefix() {
        return "TS";
    }
    public virtual string GetName() {

        return _sequenceName;
    }

    public string GetGeneratedName(int num) {
        string seqName = "";
        if (_sequenceName != null && _sequenceName.Trim().Length > 0) {
            seqName = " (" + _sequenceName.Trim() + ")";
        }
        return System.String.Format("{0}-{1:D3}{2}", this.GetPrefix(), num, seqName);
    }

    public void AssignGeneratedName(int num) {
        this.gameObject.name = GetGeneratedName(num);
    }

    public bool IsRunning() {
        return _state == SequenceState.Playing || _state == SequenceState.Finishing;
    }

    public bool Block { 
        get {
            _block = IsRunning() 
                && BeforeMaxBlock() 
                && (BeforeMinBlock() || _forceBlock);
            return _block;
        }
    }

    private bool BeforeMinBlock() {
        return useMinBlockTime && !PastTime(minBlockTime);
    }

    private bool BeforeMaxBlock() {
        return !useMaxBlockTime || !PastTime(maxBlockTime);
    }

    private bool BeforeSoftTimeLimit() {
        return !useSoftTimeLimit || !PastTime(softTimeLimit);
    }

    private bool BeforeHardTimeLimit() {
        return !useHardTimeLimit || !PastTime(hardTimeLimit);
    }




    public virtual void Play()
    {
        if (!this.enabled) {
            Debug.LogFormat("Skipping ({0})", gameObject.HierarchyName());
            return;
        }
        if (_state != SequenceState.Unplayed) {
            this.Clear();
        }
        _state = SequenceState.Playing;
        _startTime = Time.time;
        _forceBlock = true;
        _allowFinish = false;
        _allowCleanup = false;
        this.gameObject.SetActive(true);
        this.enabled = true;
        if (director != null)
            director.Play();
        onPlay.Invoke();
        Debug.LogFormat("Play({0})", gameObject.HierarchyName());
    }

    public virtual void Finish()
    {
        _state = SequenceState.Finishing;
        if (animator != null)
            animator.SetTrigger("finish");
        onFinish.Invoke();
        Debug.LogFormat("Finish({0})", gameObject.HierarchyName());
    }

    public virtual void Cleanup()
    {
        _state = SequenceState.CleanedUp;
        if (director != null)
            director.Stop();
        this.gameObject.SetActive(false);
        onCleanup.Invoke();
        Debug.LogFormat("Cleanup({0})", gameObject.HierarchyName());
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
        Debug.LogFormat("Clear({0})", gameObject.HierarchyName());
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

    #region Unity Messages
    protected virtual void Awake() {
        RunSanityChecks();
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
        if (IsRunning()) {

            // Check if we should clean up this sequence
            if ((_allowCleanup && !BeforeMinBlock() )
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

    protected virtual void OnDestroy() {
        if (IsRunning()) {
            this.Cleanup();
        }
    }

    #endregion

    private bool PastTime(float timelimit) {
        return Time.time > _startTime + timelimit;
    }


    [Tooltip("Suppress warning that there are unregistered director or animator components on this object.")]
    [FoldoutGroup("Warnings", 100000)]
    [ToggleLeft]
    public bool suppressUnregisteredComponentsWarning = false;
    [Tooltip("Suppress warning that there are unregistered director or animator components on this object.")]
    [FoldoutGroup("Warnings", 100000)]
    [ToggleLeft]
    public bool suppressPlayOnAwakeWarning = false;

    private void RunSanityChecks() {
        if (!suppressUnregisteredComponentsWarning) {
            if (director == null && TryGetComponent<PlayableDirector>(out var dir)) {
                Debug.LogWarningFormat("Sequence object ({0}) has a director component that is not registered with its sequence script", gameObject.HierarchyName());
            }
            if (animator == null && TryGetComponent<Animator>(out var anim)) {
                Debug.LogWarningFormat("Sequence object ({0}) has an animator component that is not registered with its sequence script", gameObject.HierarchyName());
            }
        }
        if (!suppressPlayOnAwakeWarning) {
            if (director != null && director.playOnAwake) {
                Debug.LogWarningFormat("Sequence object ({0}) has director set to play on awake. This can cause multiple play calls.", gameObject.HierarchyName());
            }
        }
        if (!this.enabled) {
            Debug.LogWarningFormat("Sequence object ({0}) is disabled.", gameObject.HierarchyName());
        }
    }


    [ContextMenu("Set as the current sequence")]
    public void SetAsCurrentActiveSequence() {

        Transform root = transform.root;


        // Disable the root sequence if not this
        if (root.TryGetComponent<TimedSequence>(out var rootSeq)) {
            if (rootSeq != this) {
                rootSeq.playOnAwake = false;
                rootSeq.gameObject.SetActive(false);
            }
        }

        // Disable all connected sequences
        foreach (TimedSequence sequence in root.GetComponentsInChildren<TimedSequence>(true)) {
            //Debug.Log(sequence.gameObject.HierarchyName());
            if (sequence != this) {
                sequence.playOnAwake = false;
                sequence.gameObject.SetActive(false);
            }
        }

        // Set this to active and play on awake
        this.playOnAwake = true;
        this.gameObject.SetActiveInHierarchy();
    }
    
    
    [ContextMenu("Validate Sequence")]
    public void CallValidateSequence() {
        this.ValidateSequence();
    }


    public virtual void ValidateSequence() {
        RunSanityChecks();
    }


}
