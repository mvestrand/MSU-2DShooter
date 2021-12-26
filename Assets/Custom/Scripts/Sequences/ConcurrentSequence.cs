using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class ConcurrentSequence : TimedSequence
{
    [System.Serializable]
    public struct SequenceData {
        [HorizontalGroup("Data", 0.5f)]
        [Tooltip("The sequence to play")]
        [HideLabel]
        public TimedSequence sequence;
        [HorizontalGroup("Data", 0.05f)]
        [HideLabel]
        [Tooltip("Enable or disable usage of a starting time delay")]
        public bool hasDelayTime;
        [HorizontalGroup("Data", 0.25f, LabelWidth = 60)][EnableIf("hasDelayTime")]
        [Tooltip("The time to wait before playing the sequence")]
        public float delayTime;
        [HorizontalGroup("Data", 0.25f, LabelWidth = 60)]
        [EnableIf("hasDelayTime")]
        [LabelText("Block")]
        [Tooltip("Block until this sequence has started playing")]
        public bool blockOnDelay;
    }

    [Tooltip("The sub-sequences which will play concurrently")]
    public List<SequenceData> sequences = new List<SequenceData>();

    private List<TimedSequence> running = new List<TimedSequence>();


    private int _waitingOnSequences;


    public override void Play()
    {
        base.Play();
        _waitingOnSequences = 0;
        foreach (var seq in sequences) {
            if (seq.hasDelayTime) {
                StartCoroutine(PlayWithDelay(seq.sequence, seq.delayTime, seq.blockOnDelay));
            } else {
                running.Add(seq.sequence);
                seq.sequence.Play();
            }
        }
    }


    // This function is sus. It could potentially enter a weird state if the sequence
    // is reset and played again before this subsequence plays. It could be fixed by giving
    // each call a token that it has to check is still valid before playing. I can't be bothered
    // to actually write that all right now though for a weird fringe case
    private IEnumerator PlayWithDelay(TimedSequence sequence, float delay, bool block) {
        if (block)
            _waitingOnSequences++;
        yield return new WaitForSeconds(delay);
        if (State == SequenceState.Playing) { // Do not start any more sequences if we are finishing or already done
            running.Add(sequence);
            sequence.Play();
        }
        if (block)
            _waitingOnSequences--;
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

    public override void Clear()
    {
        base.Clear();
        _waitingOnSequences = 0;
    }

    protected override void Update()
    {
        base.Update();
        RemoveFinishedSequences();
        CheckForBlockRelease();
    }

    private void CheckForBlockRelease() {
        if (_waitingOnSequences > 0 && State == SequenceState.Playing)
            return; // Block until all blocking sequences play
        foreach (var seq in sequences) {
            if (seq.sequence.Block) // Give up early if a sequence is blocking
                return;
        }
        // There is nothing blocking
        this.AllowUnblock();
    }


    private void RemoveFinishedSequences() {
        for (int i = running.Count-1; i >= 0; i--) {
            if (running[i].State == SequenceState.CleanedUp) {
                running.RemoveAt(i);
            }
        }
    }


    public override string GetPrefix() {
        return "PAR";
    }

    [ContextMenu("Grab Sub-Sequences")]
    private void GrabSequences() {
        int seqNo = 0;
        List<SequenceData> newSequences = new List<SequenceData>();
        foreach (Transform child in transform) {
            if (child.TryGetComponent<TimedSequence>(out var sequence)) {

                // Get existing sequence settings
                SequenceData seqData = new SequenceData();
                foreach (var oldSequence in sequences) {
                    if (oldSequence.sequence == sequence) {
                        seqData = oldSequence;
                        break;
                    }
                }
                seqData.sequence = sequence;
                newSequences.Add(seqData);
                sequence.AssignGeneratedName(seqNo);
                seqNo++;
            }
        }
        sequences = newSequences;
    }
}
