using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using MVest.Unity.Pooling;

/// <summary>
/// A class which controlls player aiming and shooting
/// </summary>
public class ShootingController : MonoBehaviour
{
    public enum FireMode { Single, Pattern, Sequence }

    [Header("GameObject/Component References")]
    [Tooltip("The projectile to be fired.")]
    public GameObject projectilePrefab = null;
    [Tooltip("The transform in the heirarchy which holds projectiles if any")]
    public Transform projectileHolder = null;

    public BulletPattern projectilePattern;
    public FireSequence fireSequence;
    public FireSequenceState fireSequenceState;
    public FireMode mode = FireMode.Single;




    [Header("Input")]
    [Tooltip("Whether this shooting controller is controled by the player")]
    public bool isPlayerControlled = false;

    [Header("Firing Settings")]
    [Tooltip("The minimum time between projectiles being fired.")]
    public float fireRate = 0.05f;

    [Tooltip("The maximum diference between the direction the" +
        " shooting controller is facing and the direction projectiles are launched.")]
    public float projectileSpread = 1.0f;

    // The last time this component was fired
    private float lastFired = Mathf.NegativeInfinity;
    private float lastFrameTime;

    [Header("Effects")]
    [Tooltip("The effect to create when this fires")]
    public GameObject fireEffect;

    //The input manager which manages player input
    private InputManager inputManager = null;

    /// <summary>
    /// Description:
    /// Standard unity function that runs every frame
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void Update()
    {
        ProcessInput();
    }

    /// <summary>
    /// Description:
    /// Standard unity function that runs when the script starts
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void Start()
    {
        SetupInput();
        lastFrameTime = Time.timeSinceLevelLoad;
    }

    /// <summary>
    /// Description:
    /// Attempts to set up input if this script is player controlled and input is not already correctly set up 
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void SetupInput()
    {
        if (isPlayerControlled)
        {
            if (inputManager == null)
            {
                inputManager = InputManager.instance;
            }
            if (inputManager == null)
            {
                Debug.LogError("Player Shooting Controller can not find an InputManager in the scene, there needs to be one in the " +
                    "scene for it to run");
            }
        }
    }

    /// <summary>
    /// Description:
    /// Reads input from the input manager
    /// Inputs:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    void ProcessInput()
    {
        if (isPlayerControlled) {
            UpdateFireState(inputManager.firePressed || inputManager.fireHeld);
        }
    }

    /// <summary>
    /// Description:
    /// Fires a projectile if possible
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void UpdateFireState(bool shouldFire)
    {
        float deltaTime = Time.timeSinceLevelLoad - lastFrameTime;
        lastFrameTime = Time.timeSinceLevelLoad;
        if (Time.timeScale == 0)
            return;

        if (mode == FireMode.Single || mode == FireMode.Pattern) {
            // If the cooldown is over fire a projectile
            if ((Time.timeSinceLevelLoad - lastFired) >= fireRate && Time.timeScale != 0 && shouldFire)
            {
                // Launches a projectile
                FireBaseProjectiles();

                if (fireEffect != null)
                {
                    if (fireEffect.TryGetComponent<PooledMonoBehaviour>(out var prefabPooler)) {
                        prefabPooler.Get(transform.position, transform.rotation);
                    } else {
                        Instantiate(fireEffect, transform.position, transform.rotation, null);
                    }
                }

                // Restart the cooldown
                lastFired = Time.timeSinceLevelLoad;
            }
        } else if (mode == FireMode.Sequence) {
            fireSequence.Execute(this, ref fireSequenceState, deltaTime, shouldFire);
        }
    }

    /// <summary>
    /// Description:
    /// Spawns a projectile and sets it up
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void FireBaseProjectiles(float advanceTime=0)
    {
        // Check that the prefab is valid
        if (mode == FireMode.Single && projectilePrefab != null) {
            FireSingle(advanceTime:advanceTime);
        } else if (mode == FireMode.Pattern && projectilePattern != null) {
            FirePattern(advanceTime:advanceTime);
        }
    }

    public void FireSingle(Projectile projectile=null, float advanceTime=0, bool applySpread=true, ProjectileModifiers modifiers=null) { 
        FireSingle(transform.position, transform.rotation, projectile, advanceTime, applySpread, modifiers); 
    }

    public void FireSingle(Vector3 position, Quaternion rotation, Projectile projectile = null, float advanceTime=0, bool applySpread=false, ProjectileModifiers modifiers=null) {
        // Use our stored projectile prefab if no prefab is specified
        projectile = (projectile == null ? projectilePrefab.GetComponent<Projectile>() : projectile);
        if (projectile == null)
            return;

        Quaternion spread = (applySpread ? ComputeSpread() : Quaternion.identity);
        projectile.Spawn(position, spread * rotation, projectileHolder, advanceTime, modifiers);
    }

    public void FirePattern(BulletPattern pattern = null, float advanceTime=0, bool applySpread=true, ProjectileModifiers modifiers=null) {
        FirePattern(transform.position, transform.rotation, pattern, advanceTime, applySpread, modifiers);  
    }

    public void FirePattern(Vector3 position, Quaternion rotation, BulletPattern pattern = null, float advanceTime = 0, bool applySpread = false, ProjectileModifiers modifiers=null) {
        pattern = (pattern == null ? projectilePattern : pattern);
        if (pattern == null)
            return;

        Quaternion spread = (applySpread ? ComputeSpread() : Quaternion.identity);
        pattern.Spawn(position, spread * rotation, projectileHolder, advanceTime, projectilePrefab.GetComponent<Projectile>(), modifiers);
    }

    private Quaternion ComputeSpread() {
        return Quaternion.Euler(0, 0, Random.Range(-projectileSpread, projectileSpread));
    }

    private void SpawnEffect() {
        
    }
}
