using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class TimedSequence : MonoBehaviour, ISequence
{
    [Tooltip("Play this sequence immediately")]
    [SerializeField] private bool playOnAwake = false;


    [HorizontalGroup("MinBlockTime")]
    [HideLabel][ToggleLeft]
    [Tooltip("Whether or not this sequence has a minimum block time")]
    [SerializeField] private bool useMinBlockTime = false;

    [HorizontalGroup("MinBlockTime")][EnableIf("useMinBlockTime")]
    [Tooltip("Minimum time the sequence will block for")]
    [SerializeField] private float minBlockTime = 1f;


    [HorizontalGroup("MaxBlockTime")]
    [HideLabel][ToggleLeft]
    [Tooltip("Whether or not this sequence has a maximum block time")]
    [SerializeField] private bool useMaxBlockTime = false;

    [HorizontalGroup("MaxBlockTime")][EnableIf("useMaxBlockTime")]
    [Tooltip("Maximum time the sequence will block for")]
    [SerializeField] private float maxBlockTime = 10f;


    [HorizontalGroup("SoftTimeLimit")]
    [HideLabel][ToggleLeft]
    [Tooltip("Whether or not to tell objects to end after a given time")]
    [SerializeField] private bool useSoftTimeLimit = false;

    [HorizontalGroup("SoftTimeLimit")][EnableIf("useSoftTimeLimit")]
    [Tooltip("Maximum time before telling objects to end")]
    [SerializeField] private float softTimeLimit = 10f;


    [HorizontalGroup("HardTimeLimit")]
    [HideLabel][ToggleLeft]
    [Tooltip("Whether or not to force objects to end after a given time")]
    [SerializeField] private bool useHardTimeLimit = false;

    [HorizontalGroup("HardTimeLimit")][EnableIf("useHardTimeLimit")]
    [Tooltip("Maximum time before forcing objects to end")]
    [SerializeField] private float hardTimeLimit = 20f;



    private float _startTime;

    private bool _wasPlayed = false;
    public bool WasPlayed { get { return _wasPlayed; } }
    
    public bool Block => throw new System.NotImplementedException();

    public bool Finished => throw new System.NotImplementedException();

    public bool NeedsCleanup => throw new System.NotImplementedException();

    public virtual void CheckFlag(ISequenceFlag flag, out FlagStatus status) {
        status = new FlagStatus();
    }

    public virtual void Cleanup() {

    }

    public virtual void Clear()
    {
        throw new System.NotImplementedException();
    }

    public virtual void EndEarly()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Play() {
        _startTime = Time.time;
        _wasPlayed = true;
    }

    protected virtual bool ShouldBlock() { return false; }
    protected virtual bool CanEndEarly() { return true; }
    protected virtual bool CanCleanup() { return true; }


    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }


}
