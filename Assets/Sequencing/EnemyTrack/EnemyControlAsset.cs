using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyControlAsset : PlayableAsset {
    public EnemyControlBehaviour template;
    // public ExposedReference<Enemy> prefab;
    // //    public ExposedReference<BezierSpline> spline;
    // public BezierSpline spline;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        var playable = ScriptPlayable<EnemyControlBehaviour>.Create(graph, template);
        var controller = owner.GetComponent<EnemyController>();
        controller.behaviour = playable.GetBehaviour();
        controller.behaviour.spline = owner.GetComponent<BezierSpline>();
        
        // var behaviour = playable.GetBehaviour();
        // behaviour.prefab = prefab.Resolve(graph.GetResolver());
        // behaviour.spline = spline;
        // //behaviour.spline = spline.Resolve(graph.GetResolver());
        return playable;
    }
}
