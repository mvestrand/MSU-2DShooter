using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class controls player movement
/// </summary>
public class Controller : MonoBehaviour {
    private const float Epsilon = 1E-08f;

    [Header("GameObject/Component References")]
    [Tooltip("The animator controller used to animate the player.")]
    public RuntimeAnimatorController animator = null;
    [Tooltip("The Rigidbody2D component to use in \"Astroids Mode\".")]
    public Rigidbody2D myRigidbody = null;

    [Header("Movement Variables")]
    [Tooltip("The speed at which the player will move.")]
    public float moveSpeed = 10.0f;
    [Tooltip("The speed at which the player will move while focused")]
    public float focusedMoveSpeed = 5.0f;

    public float MoveSpeed { get { return Mathf.Lerp(moveSpeed, focusedMoveSpeed, focus); } }

    [Tooltip("The speed at which the player rotates in asteroids movement mode")]
    public float rotationSpeed = 60f;

    public float focusTime = 0.2f;
    public float focus = 0f;

    public bool locked = false;

    //The InputManager to read input from
    private InputManager inputManager;

    /// <summary>
    /// Enum which stores different aiming modes
    /// </summary>
    public enum AimModes { AimTowardsMouse, AimForwards };

    [Tooltip("The aim mode in use by this player:\n" +
        "Aim Towards Mouse: Player rotates to face the mouse\n" +
        "Aim Forwards: Player aims the direction they face (doesn't face towards the mouse)")]
    public AimModes aimMode = AimModes.AimTowardsMouse;

    /// <summary>
    /// Enum to handle different movement modes for the player
    /// </summary>
    public enum MovementModes { MoveHorizontally, MoveVertically, FreeRoam, Astroids };

    [Tooltip("The movmeent mode used by this controller:\n" +
        "Move Horizontally: Player can only move left/right\n" +
        "Move Vertically: Player can only move up/down\n" +
        "FreeRoam: Player can move in any direction and can aim\n" +
        "Astroids: Player moves forward/back in the direction they are facing and rotates with horizontal input")]
    public MovementModes movementMode = MovementModes.FreeRoam;


    public bool useMoveBound = true;
    public Vector2 moveBoundMin = new Vector2(-7, -9);
    public Vector2 moveBoundMax = new Vector2(7, 9);

    // Whether the player can aim with the mouse or not
    private bool canAimWithMouse
    {
        get
        {
            return aimMode == AimModes.AimTowardsMouse;
        }
    }

    // Whether the player's X coordinate is locked (Also assign in rigidbody)
    private bool lockXCoordinate
    {
        get
        {
            return movementMode == MovementModes.MoveVertically;
        }
    }
    // Whether the player's Y coordinate is locked (Also assign in rigidbody)
    public bool lockYCoordinate
    {
        get
        {
            return movementMode == MovementModes.MoveHorizontally;
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once when the script starts before Update
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void Start()
    {
        SetupInput();
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once per frame
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    void Update()
    {
        if (locked)
            return;
        // Collect input and move the player accordingly
        HandleInput();
        // Sends information to an animator component if one is assigned
        SignalAnimator();
    }

    /// <summary>
    /// Description:
    /// Sets up the input manager if it is not already set up. Throws an error if none exists
    /// Inputs:
    /// None
    /// Returns:
    /// void
    /// </summary>
    private void SetupInput()
    {
        if (inputManager == null)
        {
            inputManager = InputManager.instance;
        }
        if (inputManager == null)
        {
            Debug.LogWarning("There is no player input manager in the scene, there needs to be one for the Controller to work");
        }
    }

    /// <summary>
    /// Description:
    /// Handles input and moves the player accordingly
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void HandleInput()
    {
        // Find the position that the player should look at
        Vector2 lookPosition = GetLookPosition();
        // Get movement input from the inputManager
        Vector3 movementVector = new Vector3(inputManager.horizontalMoveAxis, inputManager.verticalMoveAxis, 0);
        // Move the player
        MovePlayer(movementVector);
        LookAtPoint(lookPosition);
        AdjustFocus(inputManager.focusHeld);
    }

    private void AdjustFocus(bool active) {
        float maxDelta = (focusTime < Epsilon ? 1f : Time.deltaTime / focusTime);
        focus = Mathf.Clamp01(Mathf.MoveTowards(focus, (active ? 1f : 0f), maxDelta));
        
    }

    /// <summary>
    /// Description: 
    /// Handles signals to animator components
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void SignalAnimator()
    {
        // Handle Animation
        if (animator != null)
        {

        }
    }

    /// <summary>
    /// Description:
    /// Updates the position the player is looking at
    /// Inputs: 
    /// none
    /// Returns: 
    /// Vector2
    /// </summary>
    /// <returns>Vector2: The position the player should look at</returns>
    public Vector2 GetLookPosition()
    {
        Vector2 result = transform.up;
        if (aimMode != AimModes.AimForwards)
        {
            result = new Vector2(inputManager.horizontalLookAxis, inputManager.verticalLookAxis);
        }
        else
        {
            result = transform.up;
        }
        return result;
    }

    /// <summary>
    /// Description:
    /// Moves the player
    /// Inputs: 
    /// Vector3 movement
    /// Returns: 
    /// void (no return)
    /// </summary>
    /// <param name="movement">The direction to move the player</param>
    private void MovePlayer(Vector3 movement)
    {
        // Set the player's posiiton accordingly

        // Move according to astroids setting
        if (movementMode == MovementModes.Astroids)
        {

            // If no rigidbody is assigned, assign one
            if (myRigidbody == null)
            {
                myRigidbody = GetComponent<Rigidbody2D>();
            }

            // Move the player using physics
            Vector2 force = transform.up * movement.y * Time.deltaTime * MoveSpeed;
            Debug.Log(force);
            myRigidbody.AddForce(force);

            // Rotate the player around the z axis
            Vector3 newRotationEulars = transform.rotation.eulerAngles;
            float zAxisRotation = transform.rotation.eulerAngles.z;
            float newZAxisRotation = zAxisRotation - rotationSpeed * movement.x * Time.deltaTime;
            newRotationEulars = new Vector3(newRotationEulars.x, newRotationEulars.y, newZAxisRotation);
            transform.rotation = Quaternion.Euler(newRotationEulars);

        }
        // Move according to the other settings
        else
        {
            // Don't move in the x if the settings stop us from doing so
            if (lockXCoordinate)
            {
                movement.x = 0;
            }
            // Don't move in the y if the settings stop us from doing so
            if (lockYCoordinate)
            {
                movement.y = 0;
            }
            // Move the player's transform
            transform.position = transform.position + (movement * Time.deltaTime * MoveSpeed);
        }
        if (useMoveBound) {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, moveBoundMin.x, moveBoundMax.x),
                                             Mathf.Clamp(transform.position.y, moveBoundMin.y, moveBoundMax.y),
                                             transform.position.z);
        }
    }

    /// <summary>
    /// Description: 
    /// Rotates the player to look at a point
    /// Inputs: 
    /// Vector3 point
    /// Returns: 
    /// void (no return)
    /// </summary>
    /// <param name="point">The screen space position to look at</param>
    private void LookAtPoint(Vector3 point)
    {
        if (Time.timeScale > 0)
        {
            // Rotate the player to look at the mouse.
            Vector2 lookDirection = Camera.main.ScreenToWorldPoint(point) - transform.position;

            if (canAimWithMouse)
            {
                transform.up = lookDirection;
            }
            else
            {
                if (myRigidbody != null)
                {
                    myRigidbody.freezeRotation = true;
                }
            }
        }
    }
}
