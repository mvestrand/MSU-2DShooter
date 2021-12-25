using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

/// <summary>
/// A class which controls enemy behaviour
/// </summary>
public class Enemy : PooledMonoBehaviour
{
    public event System.Action<Enemy> onDeath;
    public event System.Action<Enemy> onDespawn;




    [Header("Settings")]
    [Tooltip("The max speed at which the enemy can move (m/s).")]
    public float moveSpeed = 5.0f;
    [Tooltip("The max speed at which the enemy can turn (deg / s).")]
    public float turnSpeed = 10000;
    [Tooltip("The score value for defeating this enemy")]
    public int scoreValue = 5;


    [Header("Following Settings")]
    [Tooltip("The transform of the object that this enemy should follow.")]
    public Transform followTarget = null;
    [Tooltip("The distance at which the enemy begins following the follow target.")]
    public float followRange = 10.0f;


    public EnemyController controller = null;


    [Header("Shooting")]
    [Tooltip("The enemy's gun components")]
    public List<ShootingController> guns = new List<ShootingController>();

    /// <summary>
    /// Enum to help with shooting modes
    /// </summary>
    public enum ShootMode { None, ShootAll, ShootSelect };

    [Tooltip("The way the enemy shoots:\n" +
        "None: Enemy does not shoot.\n" +
        "ShootAll: Enemy fires all guns whenever it can.")]
    public ShootMode shootMode = ShootMode.ShootAll;

    /// <summary>
    /// Enum to help wih different movement modes
    /// </summary>
    public enum MovementModes { NoMovement, FollowTarget, Scroll, PhysicsTarget, Instant, Coast, Default, NoInput };

    [Tooltip("The way this enemy will move\n" +
        "NoMovement: This enemy will not move.\n" +
        "FollowTarget: This enemy will follow the assigned target.\n" +
        "Scroll: This enemy will move in one horizontal direction only.")]
    public MovementModes movementMode = MovementModes.FollowTarget;
    public MovementModes defaultMovementMode = MovementModes.FollowTarget;

    //The direction that this enemy will try to scroll if it is set as a scrolling enemy.
    [SerializeField] private Vector3 scrollDirection = Vector3.right;


    /// <summary>
    /// Description:
    /// Standard Unity function called after update every frame
    /// Inputs: 
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void LateUpdate()
    {
        HandleBehaviour();
        // Handle despawn
        if (controller != null && controller.ShouldDespawn(transform.position)) {
            onDespawn?.Invoke(this);
            this.Release();
        }

    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first call to Update
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        if (movementMode == MovementModes.FollowTarget && followTarget == null)
        {
            if (GameManager.instance != null && GameManager.instance.player != null)
            {
                followTarget = GameManager.instance.player.transform;
            }
        }
    }

    /// <summary>
    /// Description:
    /// Handles moving and shooting in accordance with the enemy's set behaviour
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void HandleBehaviour()
    {
        // Check if the target is in range, then move
        // if (followTarget != null && (followTarget.position - transform.position).magnitude < followRange)
        // {

        MoveEnemy();
        // }
        // Attempt to shoot, according to this enemy's shooting mode
        TryToShoot();
    }

    /// <summary>
    /// Description:
    /// This is meant to be called before destroying the gameobject associated with this script
    /// It can not be replaced with OnDestroy() because of Unity's inability to distiguish between unloading a scene
    /// and destroying the gameobject from the Destroy function
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void DoBeforeDestroy()
    {
        AddToScore();
        IncrementEnemiesDefeated();
        onDeath?.Invoke(this);
    }

    /// <summary>
    /// Description:
    /// Adds to the game manager's score the score associated with this enemy if one exists
    /// Input:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    private void AddToScore()
    {
        if (GameManager.instance != null && !GameManager.instance.gameIsOver)
        {
            GameManager.AddScore(scoreValue);
        }
    }

    /// <summary>
    /// Description:
    /// Increments the game manager's number of defeated enemies
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void IncrementEnemiesDefeated()
    {
        if (GameManager.instance != null && !GameManager.instance.gameIsOver)
        {
            GameManager.instance.IncrementEnemiesDefeated();
        }       
    }

    /// <summary>
    /// Description:
    /// Moves the enemy and rotates it according to it's movement mode
    /// Inputs: none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void MoveEnemy()
    {
        UpdateMovementMode();
        switch (movementMode)
        {
            case MovementModes.Instant:
                transform.position = controller.Position;
                transform.rotation = controller.Rotation;
                break;
            case MovementModes.PhysicsTarget:
                // ---- Unimplemented ------
                break;
            case MovementModes.Coast:
            case MovementModes.FollowTarget:
                if (followTarget != null && Vector3.Distance(transform.position, followTarget.position) > followRange) {
                    float targetAngle = GetAngle((followTarget.position - transform.position).normalized);
                    float currentAngle = transform.eulerAngles.z;
                    transform.eulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(currentAngle, targetAngle, Time.deltaTime * turnSpeed));
                }
                transform.position += -transform.up * Time.deltaTime;
                break;
            default:
                // Determine correct movement
                Vector3 movement = GetDesiredMovement();

                // Determine correct rotation
                Quaternion rotationToTarget = GetDesiredRotation();

                // Move and rotate the enemy
                transform.position = transform.position + movement;
                transform.rotation = rotationToTarget;
                break;
        }
    }

    private void UpdateMovementMode()
    {

        if (controller != null)
        {
            switch (controller.movementMode)
            {
                case MovementModes.NoInput:
                    break;
                case MovementModes.Coast:
                    followTarget = null;
                    break;
                case MovementModes.FollowTarget:
                    if (followTarget == null)
                        followTarget = GameManager.instance.player?.transform;
                    break;
                case MovementModes.Default:
                    movementMode = defaultMovementMode;
                    break;
                default:
                    movementMode = controller.movementMode;
                    break;
            }
        } else {
            movementMode = defaultMovementMode;
        }
    }

    public void Attach(EnemyController controller) {
        this.controller = controller;
        controller.onFireShot += this.FireTrigger;
    }

    public void DetachController() {
        controller.onFireShot -= this.FireTrigger;
        controller = null;
    }

    public void FireTrigger() {
        switch (shootMode)
        {
            case ShootMode.None:
                break;
            case ShootMode.ShootAll:
                if (controller.ShootChannels != 0) {
                    foreach (ShootingController gun in guns)
                    {
                        gun.FireTrigger();
                    }
                }
                break;
            case ShootMode.ShootSelect:
                if (controller == null)
                    break;
                foreach (var gun in guns) {
                    if (gun.ShouldFire(controller.ShootChannels))
                        gun.FireTrigger();
                }
                break;
        }        
    }

    public float GetAngle(Vector3 target) {
        return Vector3.SignedAngle(Vector3.down, target, Vector3.forward);
    }

    /// <summary>
    /// Description:
    /// Calculates the movement of this enemy
    /// Inputs: 
    /// none
    /// Returns: 
    /// Vector3
    /// </summary>
    /// <returns>Vector3: The movement of this enemy</returns>
    protected virtual Vector3 GetDesiredMovement()
    {
        Vector3 movement;
        switch(movementMode)
        {
            case MovementModes.FollowTarget:
                movement = GetFollowPlayerMovement();
                break;
            case MovementModes.Scroll:
                movement = GetScrollingMovement();
                break;
            default:
                movement = Vector3.zero;
                break;
        }
        return movement;
    }

    /// <summary>
    /// Description:
    /// Calculates and returns the desired rotation of this enemy
    /// Inputs: 
    /// none
    /// Returns: 
    /// Quaternion
    /// </summary>
    /// <returns>Quaternion: The desired rotation</returns>
    protected virtual Quaternion GetDesiredRotation()
    {
        Quaternion rotation;
        switch (movementMode)
        {
            case MovementModes.FollowTarget:
                rotation = GetFollowPlayerRotation();
                break;
            case MovementModes.Scroll:
                rotation = GetScrollingRotation();
                break;
            default:
                rotation = transform.rotation;
                break;
        }
        return rotation;
    }

    /// <summary>
    /// Description:
    /// Tries to fire all referenced ShootingController scripts
    /// depends on shootMode variable
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void TryToShoot()
    {
        switch (shootMode)
        {
            case ShootMode.None:
                break;
            case ShootMode.ShootAll:
                if (controller == null || controller.ShootChannels != 0) {
                    foreach (ShootingController gun in guns)
                    {
                        gun.FireHeld();
                    }
                }
                break;
            case ShootMode.ShootSelect:
                if (controller == null)
                    break;
                foreach (var gun in guns) {
                    if (gun.ShouldFire(controller.ShootChannels))
                        gun.FireHeld();
                }
                break;
        }
    }

    /// <summary>
    /// Description:
    /// The direction and magnitude of the enemy's desired movement in follow mode
    /// Inputs: 
    /// none
    /// Returns: 
    /// Vector3
    /// </summary>
    /// <returns>Vector3: The movement to be used in follow movement mode.</returns>
    private Vector3 GetFollowPlayerMovement()
    {
        Vector3 moveDirection = (followTarget.position - transform.position).normalized;
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        return movement;
    }

    /// <summary>
    /// Description
    /// The desired rotation of follow movement mode
    /// Inputs: 
    /// none
    /// Returns: 
    /// Quaternion
    /// </summary>
    /// <returns>Quaternion: The rotation to be used in follow movement mode.</returns>
    private Quaternion GetFollowPlayerRotation()
    {
        float angle = Vector3.SignedAngle(Vector3.down, (followTarget.position - transform.position).normalized, Vector3.forward);
        Quaternion rotationToTarget = Quaternion.Euler(0, 0, angle);
        return rotationToTarget;
    }

    /// <summary>
    /// Description:
    /// The direction and magnitude of the enemy's desired movement in scrolling mode
    /// Inputs: 
    /// none
    /// Returns: 
    /// Vector3
    /// </summary>
    /// <returns>Vector3: The movement to be used in scrolling movement mode.</returns>
    private Vector3 GetScrollingMovement()
    {
        scrollDirection = GetScrollDirection();
        Vector3 movement = scrollDirection * moveSpeed * Time.deltaTime;
        return movement;
    }

    /// <summary>
    /// Description
    /// The desired rotation of scrolling movement mode
    /// Inputs: 
    /// none
    /// Returns: 
    /// Quaternion
    /// </summary>
    /// <returns>Quaternion: The rotation to be used in scrolling movement mode</returns>
    private Quaternion GetScrollingRotation()
    {
        return Quaternion.identity;
    }

    /// <summary>
    /// Description:
    /// Determines the direction to move in with scrolling movement mode
    /// Inputs: 
    /// none
    /// Returns: 
    /// Vector3
    /// </summary>
    /// <returns>Vector3: The desired scroll direction</returns>
    private Vector3 GetScrollDirection()
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            Vector2 screenPosition = camera.WorldToScreenPoint(transform.position);
            Rect screenRect = camera.pixelRect;
            if (!screenRect.Contains(screenPosition))
            {
                return scrollDirection * -1;
            }
        }
        return scrollDirection;
    }

    protected override void Restart() {
        if (TryGetComponent<Health>(out var health))
            health.Reset();
        foreach (var gun in guns) {
            gun.Reset();
        }
        movementMode = defaultMovementMode;

    }
}
