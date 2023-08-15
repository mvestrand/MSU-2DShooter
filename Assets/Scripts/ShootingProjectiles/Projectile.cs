using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

/// <summary>
/// A class to make projectiles move
/// </summary>
[SelectionBase]
public class Projectile : MonoBehaviour
{
    [Tooltip("The distance this projectile will move each second.")]
    public float projectileSpeed = 3.0f;

    public bool useDrag=false;
    public float dragDecel = 0;
    public Vector3 currentVelocity;
    public Vector3 dragTargetVelocity;

    public FloatCurve testCurve = new FloatCurve();

    private void FixedUpdate()
    {
        MoveProjectile(Time.fixedDeltaTime);
    }

    /// <summary>
    /// Description:
    /// Move the projectile in the direction it is heading
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void MoveProjectile(float time)
    {
        if (useDrag) {
            Vector3 steering = dragTargetVelocity - currentVelocity; 
            currentVelocity += steering.normalized * Mathf.Min(steering.magnitude, dragDecel*time);
            transform.position += currentVelocity * time;
        } else {
            // move the transform
            transform.position += transform.up * projectileSpeed * time;
        }

    }


    public void Spawn(Vector3 position, Quaternion rotation, Transform parent, float advanceTime=0, ProjectileModifiers modifiers=null, bool invert=false)  {
        float speed = projectileSpeed;

        var instance = Pool.Instantiate(gameObject, position, rotation, parent).GetComponent<Projectile>();

        if (modifiers != null) {
            (var p, var r, var inv) = modifiers.ApplyOffsets(position, rotation, invert);
            instance.transform.SetPositionAndRotation(p, r);
            speed = modifiers.ApplySpeed(speed);
        }

        instance.projectileSpeed = speed;

        if (useDrag)
            instance.currentVelocity = instance.transform.up * speed;

        if (advanceTime>0)
                instance.MoveProjectile(advanceTime);

    }
}