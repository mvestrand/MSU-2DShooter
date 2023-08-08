using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AYellowpaper;

/// <summary>
/// A class which controls enemy behaviour
/// </summary>
[SelectionBase]
public class Enemy : MonoBehaviour
{
    [SerializeField] bool overrideControls;
    [Header("Settings")]
    [Tooltip("The speed at which the enemy moves.")]
    public float moveSpeed = 5.0f;
    [Tooltip("The score value for defeating this enemy")]
    public int scoreValue = 5;

    [Header("Following Settings")]
    [Tooltip("The transform of the object that this enemy should follow.")]
    public Transform steerTarget = null;
    [Tooltip("The distance at which the enemy begins following the follow target.")]
    public float followRange = 10.0f;

    [Header("Shooting")]
    [Tooltip("The enemy's gun components")]
    public List<InterfaceReference<IShootingController, MonoBehaviour>> guns = new List<InterfaceReference<IShootingController, MonoBehaviour>>();
    

    /// <summary>
    /// Enum to help with shooting modes
    /// </summary>
    public enum ShootMode { Never, Always, OnSignal, SetByTrack, TrackAndSignal };


    [Tooltip("The way the enemy shoots:\n" +
        "None: Enemy does not shoot.\n" +
        "ShootAll: Enemy fires all guns whenever it can.")]
    public ShootMode shootMode = ShootMode.TrackAndSignal;
    public bool shouldShoot = false;

    /// <summary>
    /// Enum to help wih different movement modes
    /// </summary>
    public enum MovementModes { NoMovement, FollowTarget, Scroll};

    [Tooltip("The way this enemy will move\n" +
        "NoMovement: This enemy will not move.\n" +
        "FollowTarget: This enemy will follow the assigned target.\n" +
        "Scroll: This enemy will move in one horizontal direction only.")]
    public MovementModes movementMode = MovementModes.FollowTarget;

    //The direction that this enemy will try to scroll if it is set as a scrolling enemy.
    [SerializeField] private Vector3 scrollDirection = Vector3.right;


    public bool useTurnSpeed = false;
    public float maxTurnSpeed = 5f;
    public float turnSpeedMod = 1f;
    
    public TrackPlayer playerTracker;

    /// <summary>
    /// Description:
    /// Standard Unity function called after update every frame
    /// Inputs: 
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void Update()
    {
        HandleBehaviour();       
    }

    private void LateUpdate() {
        TryToShoot();
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
        if (movementMode == MovementModes.FollowTarget && steerTarget == null)
        {
            if (GameManager.Instance != null && GameManager.Instance.player != null)
            {
                steerTarget = GameManager.Instance.player.transform;
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
        if (!overrideControls) {
            if (steerTarget != null && (steerTarget.position - transform.position).magnitude < followRange)
            {
                MoveEnemy();
            }
            // Attempt to shoot, according to this enemy's shooting mode
        }
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
        if (GameManager.Instance != null && !GameManager.Instance.gameIsOver)
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
        if (GameManager.Instance != null && !GameManager.Instance.gameIsOver)
        {
            GameManager.Instance.IncrementEnemiesDefeated();
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
        // Determine correct movement
        Vector3 movement = GetDesiredMovement();

        // Determine correct rotation
        Quaternion rotationToTarget = GetDesiredRotation();

        // Move and rotate the enemy
        transform.position = transform.position + movement;
        transform.rotation = rotationToTarget;
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
                rotation = transform.rotation; ;
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
    public void TryToShoot()
    {
        switch (shootMode)
        {
            case ShootMode.Never:
                break;
            default:
                bool tryingToShoot = (shootMode == ShootMode.Always || 
                                        (shootMode == ShootMode.TrackAndSignal || shootMode == ShootMode.SetByTrack) && shouldShoot);
                foreach (var gun in guns)
                {
                    gun.Value.UpdateFireState(tryingToShoot, ref turnSpeedMod);                    
                }
                if (playerTracker != null) {
                    if (useTurnSpeed) {
                        playerTracker.useTurnSpeed = true;
                        playerTracker.turnSpeed = turnSpeedMod * maxTurnSpeed;
                    } else {
                        playerTracker.useTurnSpeed = false;
                    }
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
        Vector3 moveDirection = (steerTarget.position - transform.position).normalized;
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
        float angle = Vector3.SignedAngle(Vector3.down, (steerTarget.position - transform.position).normalized, Vector3.forward);
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



}
