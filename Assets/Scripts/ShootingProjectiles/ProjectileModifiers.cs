using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ProjectileModifiers {

    public enum SpeedApplyMode {
        Unused,
        Add,
        Multiply,
        Override
    }

    public Vector2 offset;
    public FloatCurve projectileDirection = new FloatCurve();
    public FloatCurve rotation = new FloatCurve();
    public bool rotationAffectsProjectile = true;
    public Vector2 rotatedOffset;
    public FloatCurve speed = new FloatCurve();
    public SpeedApplyMode speedMode = SpeedApplyMode.Unused;
    public bool invert = false;

    public (Vector3 pos, Quaternion rot, bool inv) ApplyOffsets(Vector3 rootPos, Quaternion rootRot, bool rootInv) {

        float sign = (invert == rootInv ? 1 : -1);

        // Get the randomized local rotation
        float rotationRand = (rotation.UsesT ? Random.Range(0f, 1f) : 0);
        Quaternion localRotation = Quaternion.AngleAxis(sign * rotation.Get(rotationRand, 0), Vector3.forward);

        // Get the randomized projectile direction            
        float projectileDirectionRand = (projectileDirection.UsesT ? Random.Range(0f, 1f) : 0);
        Quaternion projDirection = Quaternion.AngleAxis(sign * projectileDirection.Get(projectileDirectionRand, 0), Vector3.forward);

        // Compute the final position to spawn the projectile at
        Vector3 pos = rootPos + rootRot * new Vector3(sign * offset.x, offset.y, 0);
        pos = pos + localRotation * rootRot * new Vector3(sign * rotatedOffset.x, rotatedOffset.y, 0);

        // Compute the final rotation for the projectile
        Quaternion rot = projDirection * (rotationAffectsProjectile ? localRotation : Quaternion.identity) * rootRot;

        return (pos, rot, invert == rootInv);

    }

    public float ApplySpeed(float baseSpeed) {

        float speedRand = (speed.UsesT && speedMode != SpeedApplyMode.Unused ? Random.Range(0f, 1f) : 0);

        switch (speedMode) {
            case SpeedApplyMode.Add:
                return baseSpeed + speed.Get(speedRand, 0);
            case SpeedApplyMode.Multiply:
                return baseSpeed * speed.Get(speedRand, 1);
            case SpeedApplyMode.Override:
                return speed.Get(speedRand, baseSpeed);
            case SpeedApplyMode.Unused:
            default:
                return baseSpeed;
        }

    }
}
