using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyControlAsset : PlayableAsset {
    public ExposedReference<Enemy> prefab;
    public ExposedReference<BezierSpline> spline;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        var playable = ScriptPlayable<EnemyControlBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.prefab = prefab.Resolve(graph.GetResolver());
        behaviour.spline = spline.Resolve(graph.GetResolver());
        return playable;
    }
}
