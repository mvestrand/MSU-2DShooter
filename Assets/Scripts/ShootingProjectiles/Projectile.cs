using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

/// <summary>
/// A class to make projectiles move
/// </summary>
public class Projectile : MonoBehaviour
{
    [Tooltip("The distance this projectile will move each second.")]
    public float projectileSpeed = 3.0f;

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
        // move the transform
        transform.position += transform.up * projectileSpeed * time;
    }


    public void Spawn(Vector3 position, Quaternion rotation, Transform parent, float advanceTime=0, ProjectileModifiers modifiers=null)  {
        float speed = projectileSpeed;

        var instance = Pool.Instantiate(gameObject, position, rotation, parent).GetComponent<Projectile>();

        if (modifiers != null) {
            (var p, var r) = modifiers.ApplyOffsets(position, rotation);
            instance.transform.SetPositionAndRotation(p, r);
            speed = modifiers.ApplySpeed(speed);
        }

        instance.projectileSpeed = speed;

        if (advanceTime>0)
                instance.MoveProjectile(advanceTime);

    }
}