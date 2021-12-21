using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

/// <summary>
/// A class to make projectiles move
/// </summary>
public class Projectile : PooledMonoBehaviour
{
    [Tooltip("The distance this projectile will move each second.")]
    public float projectileSpeed = 3.0f;
    public BoundingBoxVariable despawnBoundingBox;

    /// <summary>
    /// Description:
    /// Standard Unity function called once per frame
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void Update()
    {
        MoveProjectile();
    }

    /// <summary>
    /// Description:
    /// Move the projectile in the direction it is heading
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void MoveProjectile()
    {
        // move the transform
        transform.position = transform.position + transform.up * projectileSpeed * Time.deltaTime;
    }

    private void CheckBounds() {
        if (despawnBoundingBox.Value != null && !despawnBoundingBox.Value.Contains(transform.position))
            this.Release();
    }

    protected override void Restart() { }
}