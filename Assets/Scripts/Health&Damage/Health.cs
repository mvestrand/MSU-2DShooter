using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

using MVest.Unity;
using MVest.Unity.Pooling;
using MVest.Unity.Globals;

public interface IHealth {
    /// <summary> Owning team of this health object. </summary>
    int TeamId { get; }
    /// <summary> True if the object is currently invincible. </summary>
    bool IsInvincible { get; }
    /// <summary> Current health of the object </summary>
    bool InvincibleCanDestroy { get; }
    int CurrentHealth { get; }
    /// <summary> Maximum health of the object </summary>
    int MaxHealth { get; }
    /// <summary>
    /// Deal damage to the object
    /// </summary>
    /// <param name="damageAmount">Amount of health to remove. Ignored if invincible. </param>
    void TakeDamage(int damageAmount);
    /// <summary>
    /// Event called when the object dies. The dying GameObject is given as a parameter.
    /// </summary>
    UnityGameObjectEvent OnDeath { get; }
    /// <summary>
    /// Sets health to zero and kills the game object.
    /// </summary>
    void Die();
}

/// <summary>
/// This class handles the health state of a game object.
/// 
/// Implementation Notes: 2D Rigidbodies must be set to never sleep for this to interact with trigger stay damage
/// </summary>
public class Health : MonoBehaviour, IRestartable, IHealth
{
    [Header("Team Settings")]
    [Tooltip("The team associated with this damage")]
    public int teamId = 0;
    public int TeamId { get { return teamId; } }

    [SerializeField] private bool invincibleCanDestroy = false;
    public bool InvincibleCanDestroy { get { return invincibleCanDestroy; } }

    [Header("Health Settings")]
    [Tooltip("The default health value")]
    public int defaultHealth = 1;
    [Tooltip("The maximum health value")]
    public int maximumHealth = 1;
    public int MaxHealth {get { return maximumHealth; } }
    [Tooltip("The current in game health value")]
    public int currentHealth = 1;
    public int CurrentHealth { get { return currentHealth; } }
    [Tooltip("Invulnerability duration, in seconds, after taking damage")]
    public float damageInvincibilityTime = 1f;
    [Tooltip("Invulnerability duration, in seconds, after taking damage")]
    public float respawnInvincibilityTime = 3f;
    [Tooltip("Whether or not this health is always invincible")]
    public bool isAlwaysInvincible = false;
    [Tooltip("Respawn delay, in seconds, after dying")]
    public float respawnDelayTime = 0.5f;

    [Header("Lives settings")]
    [Tooltip("Whether or not to use lives")]
    public bool useLives = false;
    [Tooltip("Current number of lives this health has")]
    public int currentLives = 3;

    public int startingLives = 3;
    [Tooltip("The maximum number of lives this health can have")]
    public int maximumLives = 5;


    public bool infiniteLives = false;

    [System.NonSerialized]
    private bool _isDying = false;

    [SerializeField]
    private GlobalInt lives;

    public UnityGameObjectEvent onDeath = new UnityGameObjectEvent();
    public UnityGameObjectEvent OnDeath { get { return onDeath; } }

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
    public bool IsInvincible {
        get { return isInvincableFromDamage || isAlwaysInvincible; }
    }

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
    IEnumerator Respawn()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
        yield return new WaitForSeconds(Mathf.Max(respawnDelayTime, 0));
        gameObject.SetActive(true);
        MakeInvincible(respawnInvincibilityTime);
        transform.position = respawnPosition;
        currentHealth = defaultHealth;
    }

    /// <summary>
    /// Description:
    /// Applies damage to the health unless the health is invincible.
    /// Inputs:
    /// int damageAmount
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="damageAmount">The amount of damage to take</param>
    public void TakeDamage(int damageAmount)
    {
        if (isInvincableFromDamage || isAlwaysInvincible)
        {
            return;
        }
        else
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation, null);
            }
            MakeInvincible(damageInvincibilityTime);
            currentHealth -= damageAmount;
            CheckDeath();
        }
    }

    private void MakeInvincible(float invincibilityTime)
    {
        if (invincibilityTime <= 0)
            return;
        timeToBecomeDamagableAgain = Time.time + invincibilityTime;
        isInvincableFromDamage = true;
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

    [Header("Effects & Polish")]
    [Tooltip("The effect to create when this health dies")]
    public GameObject deathEffect;
    [Tooltip("The effect to create when this health is damaged")]
    public GameObject hitEffect;

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
        if (_isDying) // Prevent multiple calls to die
            return;
        _isDying = true;
        onDeath.Invoke(gameObject);

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation, null);
        }

        if (useLives)
        {
            HandleDeathWithLives();
        }
        else
        {
            HandleDeathWithoutLives();
        }      
        _isDying = false;
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
        if (!infiniteLives) {
            if (lives != null) {
                lives.Value -= 1;
            } else {
                currentLives -= 1;
            }
        }

        if (infiniteLives || lives != null && lives.Value >= 0 || lives == null && currentLives >= 0) {
            GameManager.Instance.StartCoroutine(Respawn());
        }
        else
        {
            if (gameObject.tag == "Player" && GameManager.Instance != null)
            {
                gameObject.SetActive(false);
                GameManager.Instance.PlayerOutOfLives();
            } else {
                if (gameObject.GetComponent<Enemy>() != null) {
                    gameObject.GetComponent<Enemy>().DoBeforeDestroy();
                }
                Destroy(this.gameObject);
            }
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
        if (gameObject.tag == "Player" && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        if (gameObject.GetComponent<Enemy>() != null)
        {
            gameObject.GetComponent<Enemy>().DoBeforeDestroy();
        }
        Destroy(this.gameObject);
    }

    public void UseContinue() {
        currentLives = startingLives;
        GameManager.Instance.StartCoroutine(Respawn());
    }

    public void Restart(IRestartable original)
    {
        if (original is Health health) {
            currentHealth = health.currentHealth;
            defaultHealth = health.defaultHealth;
            maximumHealth = health.maximumHealth;
        }
    }
}
