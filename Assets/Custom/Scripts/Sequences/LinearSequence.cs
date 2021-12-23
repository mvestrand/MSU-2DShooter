// using System.Collections;

using System.Collections.Generic;

using UnityEngine;






public class LinearSequence : TimedSequence {

    [System.Serializable]
    public struct SequenceData {

        [Tooltip("The sequence to play")]
        public TimedSequence sequence;

    }

    public List<SequenceData> sequenceData = new List<SequenceData>();

    [Tooltip("The sub-sequences which will play in order")]
    public List<TimedSequence> sequences = new List<TimedSequence>();

    private List<TimedSequence> running = new List<TimedSequence>();

    private int curIndex;

    public override void Play()
    {
        base.Play();
        curIndex = 0;
    }

    protected override void Update() {
        base.Update();
        if (State == SequenceState.Playing) {
            AdvanceSequence();
        }
    }


//     void Update() {
//         if (_hasFinished)
//             return;

//         if (!_isEnding) {
//             if (useSoftTimeLimit && _startTime + softTimeLimit <= Time.time) // Out of time, end early
//                 Finish();
//             else
//                 AdvanceSequence();
//         }
//         CompleteFinishedSequences();
//         if (running.Count == 0) // No more running sequences, end this sequence
//             Cleanup();
//     }

//     public void Play() {
//         this.Clear();
//         _hasStarted = true;
//         _startTime = Time.time;
//         this.gameObject.SetActive(true);
//         if (sequences.Count > 0) { // Play the first sequence if we have one
//             PlaySubsequence(sequences[0]);
//             AdvanceSequence(); // Start any additional sequences if the first is non blocking
//         }
//     }

//     public void Clear() {
//         running.Clear();
//         foreach (var seq in sequences) {
//             Clear();
//         }
//         curIndex = 0;
//         _hasStarted = false;
//         _isEnding = false;
//         _hasFinished = false;
//     }

//     public void Finish() {
//         _isEnding = true;
//         foreach (var seq in running) {
//             seq.Finish();
//         }
//     }

//     public void Cleanup() {
//         foreach (var seq in running) {
//             if (seq.NeedsCleanup)
//                 seq.Cleanup();
//         }
//         running.Clear();
//         _hasFinished = true;
//         this.gameObject.SetActive(false);
//     }

//     public void CheckFlag(ISequenceFlag flag, out FlagStatus status)
//     {
//         status = new FlagStatus();
//         foreach (var seq in sequences) {
//             if (flag.ShouldInclude(seq)) {
//                 seq.CheckFlag(flag, out var subStatus);
//                 flag.CombineStatuses(ref status, subStatus);
//             }
//         }
//     }
    
    private void AdvanceSequence() {
        while (CurrentHasNoBlock()) {
            if (curIndex < sequences.Count - 1) { // Play the next sequence if there is one
                TimedSequence nextSequence = sequences[curIndex + 1];
                PlaySubsequence(nextSequence);
            }
            curIndex++;
        }
    }

    private void CheckForUnblock() {
        if (CurrentHasNoBlock()) {
            AllowUnblock();
        }
    }

    private bool CurrentHasNoBlock() {
        return curIndex < sequences.Count && !sequences[curIndex].Block;
    }

    private void PlaySubsequence(TimedSequence sequence) {
        sequence.Play();
        running.Add(sequence);
    }

    private void CompleteFinishedSequences() {
        for (int i = running.Count-1; i >= 0; i--) {
            if (running[i].State == SequenceState.CleanedUp) {
                running.RemoveAt(i);
            }
        }
    }

}
