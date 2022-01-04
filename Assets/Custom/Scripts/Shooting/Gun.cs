using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.Events;

using Sirenix.OdinInspector;

using MVest;

/// <summary>
/// A class which controlls player aiming and shooting
/// </summary>
[SelectionBase]
public class Gun : MonoBehaviour
{
    [Tooltip("Object hook for the projectile despawn box")][SerializeField] 
    private ObjectHookRef<BoundingBox> despawnBox;
    [Tooltip("The GunType object to use for this gun")]
    [SerializeField][Required] private GunType gunType;
    [Tooltip("The owning team of this gun")]
    public int teamId = 1;


    [Tooltip("Whether this shooting controller is controled by the player")]
    public bool isPlayerControlled = false;

    [Tooltip("Which controller channels mean this should fire")]
    [SerializeField] private GunChannel channel = GunChannel.Gun0;
    public GunChannel Channel { get { return channel; } }


    private GunState _state = GunState.Idle;
    public GunState State { get { return _state; } }

    // The last time this gun was fired
    private float lastFired = Mathf.NegativeInfinity; 
    //The input manager which manages player input
    private InputManager inputManager = null;

    private bool fireQueued;

    public UnityEvent onTriggerFire;

    #region Public Interface
    /// <summary>
    /// Description:
    /// Fires a projectile if possible
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void FireHeld(bool shouldFire) {
        if (gunType.fireOnTrigger)
            return;

        fireQueued = shouldFire;
        // // If the cooldown is over fire a projectile
        // if ((Time.time - lastFired) > gunType.fireRate) {
        //     FireProjectiles();
        // }
    }

    int roundsFired = 0;
    float waitUntil; // The meaning of this depends on its state
    float spoolUp = 0;

    private void UpdateFireState() {
        //Debug.LogFormat("State: {0} Queued: {1}", this.State, fireQueued);
        float deltaTime = Time.deltaTime;
        float time = Time.time;
        if (fireQueued) { // Spool up
            switch (_state) {
                case GunState.Idle:
                    if (gunType.warmupTime > 0) { // Start spooling up
                        _state = GunState.Warmup;
                        spoolUp = 0;
                        waitUntil = time + gunType.warmupTime;
                    } else {    // Start firing
                        _state = GunState.Firing;
                        waitUntil = float.NegativeInfinity;
                        roundsFired = 0;
                        UpdateFireState();
                    }
                    break;
                case GunState.Warmup:
                    spoolUp += deltaTime;
                    if (spoolUp >= gunType.warmupTime) { // Spooled up. Start firing
                        _state = GunState.Firing;
                        waitUntil = float.NegativeInfinity;
                        roundsFired = 0;
                    }
                    break;
                case GunState.Firing:
                    if (waitUntil <= time) {
                        FireProjectiles();
                        roundsFired++;
                        waitUntil = time + gunType.fireRate;
                        if (roundsFired >= gunType.maxBurstSize) {
                            if (gunType.fireOnTrigger) { // Reset the fire trigger
                                fireQueued = false;
                            }
                            if (gunType.cooldownTime.IsSet) {
                                _state = GunState.Cooldown;
                                waitUntil = time + gunType.cooldownTime;
                            } else {
                                _state = GunState.Idle;
                            }
                        }
                    }
                    break;
                case GunState.Cooldown:
                    if (waitUntil <= time) { 
                        _state = GunState.Idle;
                    }
                    break;
                default:
                    Debug.LogErrorFormat("Unknown GunState {0}", _state);
                    break;
            }
        } else { // Spool down
            switch (_state) {
                case GunState.Idle:
                    break;
                case GunState.Warmup:
                    spoolUp -= deltaTime;
                    if (spoolUp <= 0) { // Spooled down. Start idling
                        _state = GunState.Idle;
                    }
                    break;
                case GunState.Firing:
                    if (roundsFired >= gunType.minBurstSize) { // Can stop firing. Spool down
                        if (gunType.warmupTime.IsSet) {
                            spoolUp = gunType.warmupTime;
                            _state = GunState.Warmup;
                        } else {
                            _state = GunState.Idle;
                        }
                    } else if (waitUntil <= time) { // Still firing burst, keep firing
                        FireProjectiles();
                        roundsFired++;
                        waitUntil = time + gunType.fireRate;
                        if (roundsFired >= gunType.maxBurstSize) { // Finished burst
                            if (gunType.fireOnTrigger) { // Reset the fire trigger
                                fireQueued = false;
                            }
                            if (gunType.cooldownTime.IsSet) {
                                _state = GunState.Cooldown;
                                waitUntil = time + gunType.cooldownTime;
                            } else {
                                _state = GunState.Idle;
                            }
                        }
                    }
                    break;
                case GunState.Cooldown:
                    if (waitUntil <= time) { 
                        _state = GunState.Idle;
                    }
                    break;
                default:
                    Debug.LogErrorFormat("Unknown GunState {0}", _state);
                    break;
            }
        }        
    }

    public void SetFireTrigger() {
        if (gunType.fireOnTrigger) {
            //Debug.Log("TriggerFire");
            fireQueued = true;
            onTriggerFire.Invoke();
        }           // FireProjectiles();
    }

    public void Reset() {
        lastFired = Mathf.NegativeInfinity;
    }

    public bool ShouldFire(int shootChannels) {
        return ((int)channel & shootChannels) != 0;
    }
    #endregion


    #region Unity Messages

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
        // lastPosition = transform.position;
        // velocity = Vector3.zero;
    }

    protected void OnEnable() {
        gunType.RequestPreallocate();
    }

    protected void OnDisable() {
        gunType.CancelPreallocate();
    }


    /// <summary>
    /// Description:
    /// Standard unity function that runs every frame
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void FixedUpdate()
    {
        // UpdateVelocity();
        UpdateFireState();
    }

    private void LateUpdate() {
        ProcessInput();
        //Debug.LogFormat("LateUpdate[{0}]({1})", Time.frameCount, Time.realtimeSinceStartupAsDouble);
    }


    // private Vector3 lastPosition;
    // private Vector3 velocity;

    // private void UpdateVelocity() {
    //     velocity = (transform.position - lastPosition) * Time.deltaTime;
    //     lastPosition = transform.position;
    // }
    #endregion

    /// <summary>
    /// Description:
    /// Attempts to set up input if this script is player controlled and input is not already correctly set up 
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void SetupInput() {
        if (isPlayerControlled) {
            if (inputManager == null)
                inputManager = InputManager.instance;
            if (inputManager == null)
                Debug.LogError("Player Shooting Controller can not find an InputManager in the scene, there needs to be one in the " +
                    "scene for it to run");
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
    void ProcessInput() {
        if (isPlayerControlled) {
            if (gunType.fireOnTrigger) {
                if (inputManager.firePressed) {
                    SetFireTrigger();
                }
            } else {
                FireHeld(inputManager.firePressed || inputManager.fireHeld);
            }

        }
    }


    private void FireProjectiles()
    {
        // Launches a projectile
        SpawnProjectiles();
        gunType.fireEffect.Play(transform);

        // Restart the cooldown
        lastFired = Time.time;
    }


    /// <summary>
    /// Description:
    /// Spawns a projectile and sets it up
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void SpawnProjectiles() {
        gunType.SpawnProjectiles(this.transform, teamId);
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.red * 0.7f;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
        Gizmos.DrawWireCube(transform.position + transform.up * 0.25f, new Vector3(.1f, .1f, .1f));
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
        Gizmos.DrawWireCube(transform.position + transform.up * 0.25f, new Vector3(.1f, .1f, .1f));
    }


}
