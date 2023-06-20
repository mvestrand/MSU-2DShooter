using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class EnemyControlBehaviour : PlayableBehaviour {

    public Enemy prefab;
    public Enemy instance;
    public BezierSpline spline;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {

    }

    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
    }

    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
    }
}
