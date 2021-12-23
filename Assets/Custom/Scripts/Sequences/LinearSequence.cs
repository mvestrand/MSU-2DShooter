// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;



// public class LinearSequence : TimedSequence {

//     [Tooltip("Play this sequence immediately")]
//     [SerializeField] private bool playOnAwake = false;
//     [Tooltip("Whether or not this sequence has a time limit")]
//     [SerializeField] private bool useSoftTimeLimit = false;
//     [Tooltip("The time (sec) after which to end the sequence early. No more sequences will play, but it will still block on the last played sequence")]
//     [SerializeField] private float softTimeLimit = 60f;
//     [Tooltip("Whether or not this sequence should block until it is done playing")]
//     [SerializeField] private bool blockUntilFinished = true;
//     [Tooltip("The sub-sequences which will play in order")]
//     public List<ISequence> sequences = new List<ISequence>();



//     private float _startTime;
//     private int curIndex;
//     private List<ISequence> running = new List<ISequence>();
//     private bool _hasStarted = false;

//     public bool WasPlayed { get { return _hasStarted; } } 

//     public bool Block {
//         get {
//             return !CurrentHasNoBlock()                         // Block if the current sequence is blocking
//                 || (!_isEnding && curIndex != sequences.Count)  // -OR-  Block if not ending early and we aren't at the end of our sequence 
//                 || (blockUntilFinished && !_hasFinished);       // -OR-  Block if we are waiting until everything is finished
//         }
//     }

//     public bool Finished {get { return _hasFinished; } }
//     public bool NeedsCleanup { get { return _hasStarted && !_hasFinished; } }  // This cleans itself up when it finishes

//     private bool _isEnding;
//     private bool _hasFinished;

//     void Start() {
//         if (playOnAwake)
//             Play();
//     }

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
    
//     private void AdvanceSequence() {
//         while (CurrentHasNoBlock()) {
//             if (curIndex < sequences.Count - 1) { // Play the next sequence if there is one
//                 ISequence nextSequence = sequences[curIndex + 1];
//                 PlaySubsequence(nextSequence);
//             }
//             curIndex++;
//         }
//     }

//     private bool CurrentHasNoBlock() {
//         return curIndex < sequences.Count && !sequences[curIndex].Block;
//     }

//     private void PlaySubsequence(ISequence sequence) {
//         sequence.Play();
//         running.Add(sequence);
//     }

//     private void CompleteFinishedSequences() {
//         for (int i = running.Count-1; i >= 0; i--) {
//             if (running[i].Finished) {
//                 if (running[i].NeedsCleanup)
//                     running[i].Cleanup();
//                 running.RemoveAt(i);
//             }
//         }
//     }

// }
