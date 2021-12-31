using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Sirenix.OdinInspector;

using MVest;

/// <summary>
/// A class which controlls player aiming and shooting
/// </summary>
[SelectionBase]
public class Gun : MonoBehaviour
{
    [Header("GameObject/Component References")]
    [Tooltip("The projectiles to be fired.")]
    public List<Projectile> shotPrefabs = new List<Projectile>();
    public List<ProjectileData> shots = new List<ProjectileData>();


    [System.Serializable]
    public struct ProjectileData {
        [Tooltip("Which projectile to spawn")]
        public int prefabIndex;
        [Tooltip("Position offset to spawn at")]
        public Vector2 posOffset;
        [Tooltip("How much offset to give the projectile after rotating")]
        public Vector2 rotatedOffset;

        [Tooltip("How many degrees to offset the projectile")]
        [BoxGroup("Direction")]
        [HorizontalGroup("Direction/Box",0.5f)]
        [LabelText("Offset:")]
        [LabelWidth(50)]
        public float dirOffset;
        [BoxGroup("Direction")]
        [LabelWidth(50)]
        [Tooltip("The random offset from dir")][Min(0)]
        [HorizontalGroup("Direction/Box",0.5f)]
        [LabelText("Spread:")]
        public float dirSpread;

        [BoxGroup("Speed")]
        [LabelWidth(50)]
        [HorizontalGroup("Speed/Box",0.5f)]
        [LabelText("Offset:")]
        [Tooltip("How much extra speed to give the projectile")]
        public float speedOffset;
        [LabelWidth(50)]
        [HorizontalGroup("Speed/Box",0.5f)]
        [LabelText("Spread:")]
        [Tooltip("How much to randomly adjust projectile speed")]
        public float speedSpread;
    }


    public BoundingBoxReference AllowedShootingBox;

    [Header("Input")]
    [Tooltip("Whether this shooting controller is controled by the player")]
    public bool isPlayerControlled = false;

    [Tooltip("Which controller channels mean this should fire")]
    [SerializeField] private GunChannel channel = GunChannel.Gun0;
    public GunChannel Channel { get { return channel; } }
    public bool ShouldFire(int shootChannels) {
        return ((int)channel & shootChannels) != 0;
    }

    [System.Flags] public enum FireState {
        Idle = 1,
        Warmup = 2,
        Firing = 4,
        Cooldown = 8
    }

    public int teamId = 1;

    // [BoxGroup("Aim At Target While:")]
    // [HorizontalGroup("Aim At Target While:/Hor", 0.25f)]
    // [LabelText("Idle")][LabelWidth(20)]
    // public bool aimIdle = false;

    // [BoxGroup("Aim At Target While:")]
    // [HorizontalGroup("Aim At Target While:/Hor", 0.25f)]
    // [LabelText("Idle")][LabelWidth(20)]
    // public bool aimWarmup = false;

    // [BoxGroup("Aim At Target While:")]
    // [HorizontalGroup("Aim At Target While:/Hor", 0.25f)]
    // [LabelText("Idle")][LabelWidth(20)]
    // public bool aimFiring = false;

    // [BoxGroup("Aim At Target While:")]
    // [HorizontalGroup("Aim At Target While:/Hor", 0.25f)]
    // [LabelText("Idle")][LabelWidth(20)]
    // public bool aimCooldown = false;



    public FireState state = FireState.Idle;


    [Header("Firing Settings")]
    [Tooltip("The minimum time between projectiles being fired (sec)")]
    public float fireRate = 0.05f;

    // [Tooltip("How long before actually firing (sec)")]
    // public float warmUpTime = 0f;

    // [Tooltip("How long after firing a burst before we can try again (sec)")]
    // public float cooldownTime = 0f;

    // [Tooltip("Does this gun use burst fire")]
    // public bool useBurstFire = false;

    // [ShowIf("useBurstFire")]
    // [Tooltip("Minimum burst size (shots)")]
    // public int minBurst = 1;
    // [ShowIf("useBurstFire")]
    // [Tooltip("Maximum burst size (shots)")]
    // public int maxBurst = int.MaxValue;

    //private int shotsFired = 0;



    // [Tooltip("Should the current velocity of this controller be added to the projectile.")]
    // public bool addRelativeVelocity = false;



    // [Tooltip("The maximum diference between the direction the" +
    //     " shooting controller is facing and the direction projectiles are launched.")]
    // public float projectileSpread = 1.0f;

    // The last time this component was fired
    private float lastFired = Mathf.NegativeInfinity;

    [FoldoutGroup("Effects", 1000)]
    [Tooltip("The effect to create when this fires")]
    public EffectRef fireEffect;

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
        // UpdateVelocity();
        ProcessInput();
    }


    // private Vector3 lastPosition;
    // private Vector3 velocity;

    // private void UpdateVelocity() {
    //     velocity = (transform.position - lastPosition) * Time.deltaTime;
    //     lastPosition = transform.position;
    // }


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
        if (isPlayerControlled)
        {
            if (inputManager.firePressed || inputManager.fireHeld)
            {
                FireHeld();
            }
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
    public void FireHeld()
    {
        if (fireOnTrigger)
            return;

        if (!AllowedShootingBox.Value.Contains(transform.position))
            return;

        // If the cooldown is over fire a projectile
        if ((Time.time - lastFired) > fireRate)
        {
            Fire();
        }
    }

    private void Fire()
    {
        // Launches a projectile
        SpawnProjectiles();
        fireEffect.Fire(transform);

        // Restart the cooldown
        lastFired = Time.time;
    }

    public bool fireOnTrigger = false;
    public void FireTrigger() {
        if (fireOnTrigger)
            Fire();
    }




    /// <summary>
    /// Description:
    /// Spawns a projectile and sets it up
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void SpawnProjectiles()
    {
        foreach (var shot in shots) {
            float z = Random.Range(-shot.dirSpread, shot.dirSpread) + shot.dirOffset;
            Vector3 pos = transform.TransformDirection((Vector3)shot.posOffset);


            Projectile shotObj = shotPrefabs[shot.prefabIndex].Get<Projectile>(transform.position, transform.rotation);
            shotObj.transform.position += pos;
            shotObj.transform.eulerAngles += new Vector3(0,0,z);
            shotObj.transform.position += shotObj.transform.TransformVector((Vector3)shot.rotatedOffset);
            shotObj.projectileSpeed += shot.speedOffset + Random.Range(-shot.speedSpread, shot.speedSpread);

            shotObj.TryGetComponent<Damage>(out var damage);
            damage.teamId = this.teamId;
        }
    }

    public void Reset() {
        lastFired = Mathf.NegativeInfinity;
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
