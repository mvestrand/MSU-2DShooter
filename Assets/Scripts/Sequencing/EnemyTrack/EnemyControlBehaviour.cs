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
    public bool shouldShoot;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        Enemy enemy = playerData as Enemy;
        if (enemy == null || spline == null)
            return;

        float u = spline.ArcLengthParameter(t);
        enemy.transform.position = spline.GetPoint(u);

        float desiredRotation = 0;
        float unusedRotationWeight = 1;

        // Specified direction
        desiredRotation += AllocateWeight(ref unusedRotationWeight, Mathf.Clamp01(directionWeight)) * direction;

        // Movement direction
        desiredRotation += AllocateWeight(ref unusedRotationWeight, Mathf.Clamp01(moveDirectionWeight)) * 
                            Vector3.SignedAngle(Vector3.down, spline.GetVelocity(u).normalized, Vector3.forward);

        // Player track direction
        if (GameManager.Instance != null && GameManager.Instance.player != null) {

            var steerTarget = GameManager.Instance.player.transform;
            var targetDirection = new Vector3(steerTarget.position.x - enemy.transform.position.x, steerTarget.position.y - enemy.transform.position.y, 0).normalized;
            float angle = Vector3.SignedAngle(Vector3.down, targetDirection, Vector3.forward);
            desiredRotation += AllocateWeight(ref unusedRotationWeight, playerTrackWeight) * angle;
        }

        enemy.transform.eulerAngles = new Vector3(0, 0, desiredRotation);
        enemy.shouldShoot = shouldShoot;
    }

    private static float AllocateWeight(ref float unusedWeight, float desiredWeight) {
        float weight = Mathf.Min(unusedWeight, Mathf.Clamp01(desiredWeight));
        unusedWeight = Mathf.Max(unusedWeight - desiredWeight, 0);
        return weight;
    }
}
