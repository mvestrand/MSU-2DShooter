#if UNITY_EDITOR
using System.ComponentModel;
#endif
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

using MVest.Unity.Globals;

/// <summary>
/// Playable Asset class for Activation Tracks
/// </summary>
#if UNITY_EDITOR
[DisplayName("Conditional Activation Clip")]
#endif
class ConditionalActivationPlayableAsset : PlayableAsset, ITimelineClipAsset
{

    [SerializeField] GlobalBool condition;

    /// <summary>
    /// Returns a description of the features supported by activation clips
    /// </summary>
    public ClipCaps clipCaps { get { return ClipCaps.None; } }

    /// <summary>
    /// Overrides PlayableAsset.CreatePlayable() to inject needed Playables for an activation asset
    /// </summary>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<ConditionalActivationControlPlayable>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.condition = condition;

        return playable;
        //return Playable.Create(graph);
    }
}
