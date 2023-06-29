using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}