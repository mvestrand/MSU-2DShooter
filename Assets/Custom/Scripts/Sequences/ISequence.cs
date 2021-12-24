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
    void Clear(); // Reset all runtime state to initial values
}


