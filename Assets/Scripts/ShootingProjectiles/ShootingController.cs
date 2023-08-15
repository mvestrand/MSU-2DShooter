using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using MVest.Unity.Pooling;

public interface IShootingController {
    void PlayOneShot();
    bool UpdateFireState(bool shouldFire, ref float turnSpeedLimit);
    bool UpdateFireState(bool shouldFire, float deltaTime, float extraTime, ref float turnSpeedLimit);
    float TimeToNextEvent(bool shouldFire);
}


/// <summary>
/// A class which controlls player aiming and shooting
/// </summary>
public class ShootingController : MonoBehaviour, IShootingController
{
    public const float MinFireTime = 0.001f;


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

    public int channel = 0;

    public bool invert = false;

    public bool IsRunning {
        get {
            return mode == FireMode.Sequence && fireSequenceState.IsRunning;
        }
    }


    [Header("Input")]
    [Tooltip("Whether this shooting controller is controled by the player")]
    public bool isPlayerControlled = false;

    [Header("Firing Settings")]
    [Tooltip("The minimum time between projectiles being fired.")]
    public float fireRate = 0.05f;
    public FloatCurve fireRateAdv = new FloatCurve(FloatCurveMode.Disabled);

    [Tooltip("The maximum diference between the direction the" +
        " shooting controller is facing and the direction projectiles are launched.")]
    public float projectileSpread = 1.0f;

    private float fireCooldown = 0;

    // The last time this component was fired
    // private float lastFired = Mathf.NegativeInfinity;
    // private float lastFrameTime;

    [Header("Effects")]
    [Tooltip("The effect to create when this fires")]
    public GameObject fireEffect;

    //The input manager which manages player input
    private InputManager inputManager = null;

    protected void Update()
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
    protected void Start()
    {
        SetupInput();
    }

    protected void OnEnable() {
        // lastFrameTime = Time.timeSinceLevelLoad;
        // lastFired = Mathf.NegativeInfinity;
        fireSequenceState.Stop();
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
            float dummy = 0;
            UpdateFireState(inputManager.firePressed || inputManager.fireHeld, Time.deltaTime, 0, ref dummy);
        }
    }

    public void PlayOneShot() {
        if (mode == FireMode.Single || mode == FireMode.Pattern) {
            FireBaseProjectiles();
            PlayEffect();
            fireCooldown = fireRate;
            // lastFired = Time.timeSinceLevelLoad;
        } else if (mode == FireMode.Sequence){
            fireSequenceState.Start();
        }
    }


    public bool UpdateFireState(bool shouldFire, ref float turnSpeedLimit) {
        return UpdateFireState(shouldFire, Time.deltaTime, 0, ref turnSpeedLimit);
    }

    /// <summary>
    /// Description:
    /// Fires a projectile if possible
    /// Inputs: 
    /// none
    /// Returns: 
    /// True if still firing, false otherwise. Always matches shouldFire, EXCEPT when running a fire sequence
    /// </summary>
    public bool UpdateFireState(bool shouldFire, float deltaTime, float extraTime, ref float turnSpeedLimit)
    {
        //float deltaTime = Time.timeSinceLevelLoad - lastFrameTime;
        // lastFrameTime = Time.timeSinceLevelLoad;
        if (Time.timeScale == 0)
            return shouldFire;

        if (mode == FireMode.Single || mode == FireMode.Pattern) {
            if (shouldFire || !fireRateAdv.Enabled)
                fireCooldown -= deltaTime;
            // If the cooldown is over fire a projectile
            if (fireCooldown <= 0 && Time.timeScale != 0 && shouldFire) {
                // Launches a projectile
                FireBaseProjectiles();
                PlayEffect();

                // Restart the cooldown
                float rand = (fireRateAdv.UsesT ? Random.Range(0f, 1f) : 0);
                fireCooldown = Mathf.Max(fireRateAdv.Get(rand, fireRate), MinFireTime);
            }
        } else if (mode == FireMode.Sequence) {
            return fireSequence.Execute(this, ref fireSequenceState, deltaTime, extraTime, shouldFire, ref turnSpeedLimit);
        }
        return shouldFire;
    }

    private void PlayEffect()
    {
        if (fireEffect != null)
        {
            if (fireEffect.TryGetComponent<PooledMonoBehaviour>(out var prefabPooler))
            {
                prefabPooler.Get(transform.position, transform.rotation);
            }
            else
            {
                Instantiate(fireEffect, transform.position, transform.rotation, null);
            }
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
        projectile.Spawn(position, spread * rotation, projectileHolder, advanceTime, modifiers, invert);
    }

    public void FirePattern(BulletPattern pattern = null, float advanceTime=0, bool applySpread=true, ProjectileModifiers modifiers=null) {
        FirePattern(transform.position, transform.rotation, pattern, advanceTime, applySpread, modifiers);  
    }

    public void FirePattern(Vector3 position, Quaternion rotation, BulletPattern pattern = null, float advanceTime = 0, bool applySpread = false, ProjectileModifiers modifiers=null) {
        pattern = (pattern == null ? projectilePattern : pattern);
        if (pattern == null)
            return;

        Quaternion spread = (applySpread ? ComputeSpread() : Quaternion.identity);
        pattern.Spawn(position, spread * rotation, projectileHolder, advanceTime, (projectilePrefab!=null ? projectilePrefab.GetComponent<Projectile>() : null), modifiers, invert);
    }

    private Quaternion ComputeSpread() {
        return Quaternion.Euler(0, 0, Random.Range(-projectileSpread, projectileSpread));
    }

    public float TimeToNextEvent(bool shouldFire)
    {
        if (mode == FireMode.Single || mode == FireMode.Pattern) {
            return (shouldFire ? fireCooldown : float.MaxValue);
        }
        else if (mode == FireMode.Sequence)
            return fireSequence.TimeToNextEvent(fireSequenceState, shouldFire);
        return float.MaxValue;
    }


    // public void OnDrawGizmos() {
    //     Gizmos.color = (isPlayerControlled ? new Color(1, .5f, 0, .5f) : new Color(1, 0, 0, .5f));
    //     Gizmos.DrawLine(transform.position, transform.position + 20 * transform.up);
    // }

}
