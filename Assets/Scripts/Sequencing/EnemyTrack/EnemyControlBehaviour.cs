using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

using MVest.Unity.Pooling;

[System.Serializable]
public class EnemyControlBehaviour : PlayableBehaviour {

    [System.NonSerialized] public BezierSpline spline;
    public float t;
    public float direction;
    [Range(0, 1)] public float moveDirectionWeight;
    public bool shouldTrackPlayer;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        Enemy enemy = playerData as Enemy;
        if (enemy == null || spline == null)
            return;
        float desiredRotation = 0;
        float u = spline.ArcLengthParameter(t);


        desiredRotation += Vector3.SignedAngle(Vector3.down, spline.GetVelocity(u).normalized, Vector3.forward) * moveDirectionWeight;
        desiredRotation += (1 - moveDirectionWeight) * direction;

        enemy.transform.position = spline.GetPoint(u);
        enemy.transform.eulerAngles = new Vector3(0, 0, desiredRotation);
    }

}
