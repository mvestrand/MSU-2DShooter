using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

using MVest.Unity.Pooling;

[System.Serializable]
public class EnemyControlBehaviour : PlayableBehaviour {

    [System.NonSerialized] public BezierSpline spline;
    [System.NonSerialized] public BezierSpline spline2;
    [Range(0, 1)] public float splineWeight;
    public float t;
    public float direction;
    [Range(0, 1)] public float directionWeight;
    [Range(0, 1)] public float moveDirectionWeight;
    [Range(0, 1)] public float playerTrackWeight;
    public bool shouldShoot;
    public bool useTurnSpeed = false;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        Enemy enemy = playerData as Enemy;
        if (enemy == null || spline == null)
            return;

        float u = spline.ArcLengthParameter(t);
        float u2 = (spline2 != null ? spline2.ArcLengthParameter(t) : 0);
        enemy.transform.position = InterpSplinePosition(u, u2);

        float desiredRotation = 0;
        float unusedRotationWeight = 1;

        // Specified direction
        desiredRotation += AllocateWeight(ref unusedRotationWeight, Mathf.Clamp01(directionWeight)) * direction;

        // Movement direction
        desiredRotation += AllocateWeight(ref unusedRotationWeight, Mathf.Clamp01(moveDirectionWeight)) * 
                            Vector3.SignedAngle(Vector3.down, InterpSplineVelocity(u, u2).normalized, Vector3.forward);

        // Player track direction
        if (GameManager.Instance != null && GameManager.Instance.player != null) {

            var steerTarget = GameManager.Instance.player.transform;
            var targetDirection = new Vector3(steerTarget.position.x - enemy.transform.position.x, steerTarget.position.y - enemy.transform.position.y, 0).normalized;
            float angle = Vector3.SignedAngle(Vector3.down, targetDirection, Vector3.forward);
            desiredRotation += AllocateWeight(ref unusedRotationWeight, playerTrackWeight) * angle;
        }

        if (enemy.useTurnSpeed && useTurnSpeed) {
            float currentRotation = enemy.transform.eulerAngles.z;
            desiredRotation = Mathf.MoveTowardsAngle(currentRotation, desiredRotation, Time.deltaTime * enemy.turnSpeedMod*enemy.maxTurnSpeed);
        }

        enemy.transform.eulerAngles = new Vector3(0, 0, desiredRotation);
        enemy.shouldShoot = shouldShoot;
    }

    private Vector3 InterpSplineVelocity(float u, float u2) {
        if (spline2 == null)
            return spline.GetVelocity(u);
        return (1 - splineWeight) * spline.GetVelocity(u) + splineWeight * spline2.GetVelocity(u2);
    }

    private Vector3 InterpSplinePosition(float u, float u2) {
        if (spline2 == null)
            return spline.GetPoint(u);
        return (1 - splineWeight) * spline.GetPoint(u) + splineWeight * spline2.GetPoint(u2);
    }

    private static float AllocateWeight(ref float unusedWeight, float desiredWeight) {
        float weight = Mathf.Min(unusedWeight, Mathf.Clamp01(desiredWeight));
        unusedWeight = Mathf.Max(unusedWeight - desiredWeight, 0);
        return weight;
    }
}
