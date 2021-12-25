// using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;


public class LinearSequence : TimedSequence {

    [System.Serializable]
    public struct SequenceData {
        [HorizontalGroup("Data", 0.5f)]
        [Tooltip("The sequence to play")]
        [HideLabel]
        public TimedSequence sequence;
        [HorizontalGroup("Data", 0.05f)]
        [HideLabel]
        [Tooltip("Enable or disable usage of a skip time")]
        public bool hasSkipTime;
        [HorizontalGroup("Data", 0.5f, LabelWidth = 60)][EnableIf("hasSkipTime")]
        [Tooltip("The latest time to play at, after which the sequence is skipped")]
        public float skipTime;        
    }

    [Tooltip("The sub-sequences which will play in order")]
    public List<SequenceData> sequences = new List<SequenceData>();

    private List<TimedSequence> running = new List<TimedSequence>();

    private int curIndex;

    public override void Play()
    {
        base.Play();
        curIndex = 0;
        if (sequences.Count > 0) {
            sequences[0].sequence.Play();

        }
    }

    public override void Finish() {
        foreach (var seq in running) {
            seq.Finish();
        }
    }

    public override void Cleanup() {
        foreach (var seq in running) {
            seq.Cleanup();
        }
        running.Clear();
    }


    public override void Clear() {
        curIndex = 0;
    }
    
    protected override void Update() {
        if (this.IsRunning()) {
            base.Update();
            if (State == SequenceState.Playing) {
                AdvanceSequence();
            }
            //     CheckForUnblock();
            //     RemoveFinishedSequences();
            //     if (running.Count == 0) {
            //         Cleanup();
            //     }
        }
    }


    private void AdvanceSequence() {
        while (curIndex < sequences.Count && CurrentHasNoBlock()) {
            if (curIndex < sequences.Count - 1) { // Play the next sequence if there is one
                PlaySubsequence(sequences[curIndex + 1]);
            }
            curIndex++;
        }
    }

    // private void CheckForUnblock() {
    //     if (CurrentHasNoBlock() && (State == SequenceState.Finishing || curIndex >= sequences.Count)) {
    //         AllowUnblock();
    //     }
    // }

    private bool CurrentHasNoBlock() {
        return curIndex >= sequences.Count || !sequences[curIndex].sequence.Block;
    }

    private void PlaySubsequence(in SequenceData sequence) {
        if (sequence.hasSkipTime && Time.time > StartTime + sequence.skipTime)
            return;
        sequence.sequence.Play();
        running.Add(sequence.sequence);
    }

    private void RemoveFinishedSequences() {
        for (int i = running.Count-1; i >= 0; i--) {
            if (running[i].State == SequenceState.CleanedUp) {
                running.RemoveAt(i);
            }
        }
    }



}
