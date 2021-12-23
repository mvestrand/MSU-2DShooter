using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum SequenceState {
    Unplayed,
    Playing,
    Finishing,
    CleanedUp
}

public interface ISequence {
    SequenceState State { get; } // Current run state of the sequence
    bool Block { get; } // Should the sequence block other sequences
    void Play(); // Start the sequence: spawn/activate objects, begin timers
    void Finish(); // Force the sequence to start ending: move objects off screen, play effects
    void Cleanup(); // Clean up after the sequence: despawn/deactivate objects
    void CheckFlag(ISequenceFlag flag, out FlagStatus status);
    void Clear(); // Reset all runtime state to initial values
}



public interface ISequenceFlag {
    void CombineStatuses(ref FlagStatus a, in FlagStatus b);
    bool ShouldInclude(ISequence sequence);
}

public struct FlagStatus {
    public bool isNonNull;
    public bool set;
    public int iValue;
    public float fValue;

}

