using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

/// <summary>
/// A class to make projectiles move
/// </summary>
[SelectionBase]
public class Projectile : PooledMonoBehaviour
{
    [Tooltip("The distance this projectile will move each second.")]
    public float projectileSpeed = 3.0f;
    public float turnSpeed = 0.0f;
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
        CheckBounds();
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
        transform.eulerAngles += new Vector3(0, 0, turnSpeed * Time.deltaTime);
        transform.position = transform.position + transform.up * projectileSpeed * Time.deltaTime;
    }

    private void CheckBounds() {
        if (despawnBoundingBox != null && despawnBoundingBox.Value != null && !despawnBoundingBox.Value.Contains(transform.position))
            this.Release();
    }

    public override void Restart() {}

}