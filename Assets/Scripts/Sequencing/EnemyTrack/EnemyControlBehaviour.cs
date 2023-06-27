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
    [Range(0, 1)] public float directionWeight;
    [Range(0, 1)] public float moveDirectionWeight;
    [Range(0, 1)] public float playerTrackWeight;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        Enemy enemy = playerData as Enemy;
        if (enemy == null || spline == null)
            return;

        float u = spline.ArcLengthParameter(t);
        enemy.transform.position = spline.GetPoint(u);

        float desiredRotation = 0;
        float unusedRotationWeight = 1;

        // Specified direction
        desiredRotation += AllocateWeight(ref unusedRotationWeight, directionWeight) * direction;

        // Movement direction
        desiredRotation += AllocateWeight(ref unusedRotationWeight, moveDirectionWeight) * 
                            Vector3.SignedAngle(Vector3.down, spline.GetVelocity(u).normalized, Vector3.forward);

        // Player track direction
        if (GameManager.Instance != null && GameManager.Instance.player != null) {
            var steerTarget = GameManager.Instance.player.transform;
            float angle = Vector3.SignedAngle(Vector3.down, (steerTarget.position - enemy.transform.position).normalized, Vector3.forward);
            desiredRotation += AllocateWeight(ref unusedRotationWeight, playerTrackWeight) * angle;
        }

        enemy.transform.eulerAngles = new Vector3(0, 0, desiredRotation);
    }

    private static float AllocateWeight(ref float unusedWeight, float desiredWeight) {
        float weight = Mathf.Min(unusedWeight, desiredWeight);
        unusedWeight = Mathf.Max(unusedWeight - desiredWeight, 0);
        return weight;
    }
}
