using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

using Sirenix.OdinInspector;

using MVest;

/// <summary>
/// This class handles the health state of a game object.
/// 
/// Implementation Notes: 2D Rigidbodies must be set to never sleep for this to interact with trigger stay damage
/// </summary>
public class Health : MonoBehaviour
{
    private static WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

    [Header("Team Settings")]
    [Tooltip("The team associated with this health")]
    [SerializeField] private int _teamId;
    public int TeamId { get { return _teamId; } }

    [Header("Health Settings")]
    [Tooltip("The default health value")]
    public float defaultHealth = 1;
    [Tooltip("The maximum health value")]
    public float maximumHealth = 1;
    [Tooltip("The current in game health value")]
    public float currentHealth = 1;
    [Tooltip("Invulnerability duration, in seconds, after taking damage")]
    public float invincibilityTime = 3f;
    [Tooltip("Whether or not this health is always invincible")]
    public bool isAlwaysInvincible = false;

    [Header("Lives settings")]
    [Tooltip("Whether or not to use lives")]
    public bool useLives = false;
    [Tooltip("Current number of lives this health has")]
    public int currentLives = 3;
    [Tooltip("The maximum number of lives this health can have")]
    public int maximumLives = 5;

    [Tooltip("Elemental type modifier of this health")]
    public ElementModifier damageModifier;
    [Tooltip("A shield that damage can be deferred to")]
    public Shield shield;


    /// <summary>
    /// Description:
    /// Standard unity funciton called before the first frame update
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetRespawnPoint(transform.position);
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
        InvincibilityCheck();
    }

    // The specific game time when the health can be damged again
    private float timeToBecomeDamagableAgain = 0;
    // Whether or not the health is invincible
    private bool isInvincableFromDamage = false;

    /// <summary>
    /// Description:
    /// Checks against the current time and the time when the health can be damaged again.
    /// Removes invicibility if the time frame has passed
    /// Inputs:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    private void InvincibilityCheck()
    {
        if (timeToBecomeDamagableAgain <= Time.time)
        {
            isInvincableFromDamage = false;
        }
    }

    // The position that the health's gameobject will respawn at if lives are being used
    private Vector3 respawnPosition;
    /// <summary>
    /// Description:
    /// Changes the respawn position to a new position
    /// Inputs:
    /// Vector3 newRespawnPosition
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="newRespawnPosition">The new position to respawn at</param>
    public void SetRespawnPoint(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    /// <summary>
    /// Description:
    /// Repositions the health's game object to the respawn position and resets the health to the default value
    /// Inputs:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    void Respawn()
    {
        transform.position = respawnPosition;
        currentHealth = defaultHealth;
    }

    public void HandleDamage(Damage damage) {
        if (shield == null || !shield.IsActive || damage.ignoreShield) {
            // TODO fix this to allow for shields that don't provide full coverage
            //  --  This could maybe be done with raytracing?
            // -OR- check if the damage hits the health object but not the shield object
            //      using WaitForFixed to make the health be checked last
            if (TakeDamage(damage.damageAmount * damageModifier.Modify(damage.element))) {
                damage.NotifyHit();
            }
        }
    }

    // private IEnumerator WaitForShieldCollision(Damage damage) {
    //     yield return waitForFixed;
    //     if (damage == null) { // The damage object no longer exists
    //         // Does this never happen? or often happen?
    //         Debug.Log("Damage object no longer exists");
    //         yield break;
    //     }
    // }


    /// <summary>
    /// Description:
    /// Applies damage to the health unless the health is invincible.
    /// Inputs:
    /// int damageAmount
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="damageAmount">The amount of damage to take</param>
    public bool TakeDamage(float damage)
    {
        if (isInvincableFromDamage || isAlwaysInvincible)
        {
            return false;
        }
        else
        {
            hitEffect.Fire(transform);
            // if (hitEffect != null)
            // {
            //     Instantiate(hitEffect, transform.position, transform.rotation, null);
            // }
            timeToBecomeDamagableAgain = Time.time + invincibilityTime;
            isInvincableFromDamage = true;
            currentHealth -= damage;
            CheckDeath();
            return true;
        }
    }

    /// <summary>
    /// Description:
    /// Applies healing to the health, capped out at the maximum health.
    /// Inputs:
    /// int healingAmount
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="healingAmount">How much healing to apply</param>
    public void ReceiveHealing(int healingAmount)
    {
        currentHealth += healingAmount;
        if (currentHealth > maximumHealth)
        {
            currentHealth = maximumHealth;
        }
        CheckDeath();
    }

    [FoldoutGroup("Effects", 1000)]
    [Tooltip("The effect to create when this health dies")]
    public EffectRef deathEffect;
    [FoldoutGroup("Effects", 1000)]
    [Tooltip("The effect to create when this health is damaged")]
    public EffectRef hitEffect;

    /// <summary>
    /// Description:
    /// Checks if the health is dead or not. If it is, true is returned, false otherwise.
    /// Calls Die() if the health is dead.
    /// Inputs:
    /// none
    /// Returns:
    /// bool
    /// </summary>
    /// <returns>Bool: true or false value representing if the health has died or not (true for dead)</returns>
    bool CheckDeath()
    {
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Description:
    /// Handles the death of the health. If a death effect is set, it is created. If lives are being used, the health is respawned.
    /// If lives are not being used or the lives are 0 then the health's game object is destroyed.
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public void Die()
    {
        deathEffect.Fire(transform);
        // if (deathEffect != null)
        // {
        //     Instantiate(deathEffect, transform.position, transform.rotation, null);
        // }

        if (useLives)
        {
            HandleDeathWithLives();
        }
        else
        {
            HandleDeathWithoutLives();
        }      
    }

    /// <summary>
    /// Description:
    /// Handles the death of the health when lives are being used
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void HandleDeathWithLives()
    {
        currentLives -= 1;
        if (currentLives > 0)
        {
            Respawn();
        }
        else
        {
            if (gameObject.tag == "Player" && GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
            if (gameObject.GetComponent<Enemy>() != null)
            {
                gameObject.GetComponent<Enemy>().DoBeforeDestroy();
            }
            gameObject.DestroyPooled();
        }
    }
    /// <summary>
    /// Description:
    /// Handles death when lives are not being used
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void HandleDeathWithoutLives()
    {
        if (gameObject.tag == "Player" && GameManager.instance != null)
        {
            GameManager.instance.GameOver();
            gameObject.DestroyPooled();
        }
        else if (gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.DoBeforeDestroy();
            enemy.Release();
        }
    }

    /// <summary>
    /// Called when a projectile grazes this object without damaging it
    /// </summary>
    public void OnGraze() {

    }

    public void Reset() {
        currentHealth = defaultHealth;
    }
}
