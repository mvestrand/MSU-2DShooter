using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class EnemyControlClip : PlayableAsset , ITimelineClipAsset {
    public ExposedReference<BezierSpline> spline;
    public EnemyControlBehaviour template;
    public ClipCaps clipCaps { get { return ClipCaps.None; } }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        var playable = ScriptPlayable<EnemyControlBehaviour>.Create(graph, template);
        var behaviour = playable.GetBehaviour();
        behaviour.spline = spline.Resolve(graph.GetResolver());
        return playable;
    }

}
