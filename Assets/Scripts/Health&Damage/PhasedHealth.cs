using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity;
using MVest.Unity.Globals;

[System.Serializable]
public class HealthPhase {
    public int currentHealth = 1;
    public int maxHealth = 1;
    public float phaseTime = 30f;
    public bool extraTimeCarriesOver = true;
    public int phaseScore = 100;
    public int phaseTimeBonus = 100;
    public float maxExtraTimeBonus = 30f;
    public int bonusHealth = 0;
}

public class PhasedHealth : MonoBehaviour, IHealth
{
    public List<HealthPhase> phases = new List<HealthPhase>();


    public UnityIntegerEvent onPhaseTimeout = new UnityIntegerEvent();
    public UnityIntegerEvent onPhaseBeaten = new UnityIntegerEvent();
    public UnityIntegerEvent onPhaseStart = new UnityIntegerEvent();
    public UnityIntegerEvent onPhasePreStart = new UnityIntegerEvent();


    private UnityGameObjectEvent _onDeath = new UnityGameObjectEvent();
    public UnityGameObjectEvent OnDeath { get { return _onDeath; } }
    private UnityGameObjectEvent _onRemove = new UnityGameObjectEvent();
    public UnityGameObjectEvent OnRemove { get { return _onRemove; } }

    // [SerializeField] private Animator animator;
    // [SerializeField] private string animatorPhaseBeatenTrigger = "PhaseBeaten";
    // [SerializeField] private string animatorPhaseTimeoutTrigger = "PhaseTimeout";

    // public bool ControlledByAnimator { get { return animator != null; } }


    public int TeamId { get { return teamId; } }

    public bool IsInvincible { get { return invincible || waitingOnPhaseChange; } }

    public int CurrentHealth { get { return (currentPhase >= 0 && currentPhase < phases.Count ? phases[currentPhase].currentHealth : 0); } }

    public int MaxHealth { get { return (currentPhase >= 0 && currentPhase < phases.Count ? phases[currentPhase].maxHealth : 0); } }


    public float timeLeft = 0;

    int currentPhase = 0;

    [Header("Team Settings")]
    [Tooltip("The team associated with this damage")]
    public int teamId = 1;
    public bool invincible = false;
    public bool invincibleCanDestroy = true;
    public bool InvincibleCanDestroy { get { return invincibleCanDestroy; } }
    private bool waitingOnPhaseChange = true;


    [Header("Effects & Polish")]
    [Tooltip("The effect to create when this health dies")]
    public GameObject deathEffect;
    [Tooltip("The effect to create when this health is damaged")]
    public GameObject hitEffect;

    [Tooltip("The effect to create when this health hit but not damaged")]
    public GameObject invincibleHitEffect;

    private bool _isEnding = false;



    // /// <summary>
    // /// Description:
    // /// Standard Unity function called once per frame
    // /// Inputs:
    // /// none
    // /// Returns:
    // /// void (no return)
    // /// </summary>
    // void Update()
    // {
    //     InvincibilityCheck();
    // }

    // // The specific game time when the health can be damged again
    // private float timeToBecomeDamagableAgain = 0;
    // // Whether or not the health is invincible
    // private bool isInvincableFromDamage = false;
    // public bool IsInvincible {
    //     get { return isInvincableFromDamage || isAlwaysInvincible; }
    // }

    // /// <summary>
    // /// Description:
    // /// Checks against the current time and the time when the health can be damaged again.
    // /// Removes invicibility if the time frame has passed
    // /// Inputs:
    // /// None
    // /// Returns:
    // /// void (no return)
    // /// </summary>
    // private void InvincibilityCheck()
    // {
    //     if (timeToBecomeDamagableAgain <= Time.time)
    //     {
    //         isInvincableFromDamage = false;
    //     }
    // }

    int GetTotalMaxHealth() {
        int total = 0;
        foreach (var phase in phases) {
            total += phase.maxHealth;
        }
        return total;
    }

    int GetReserveHealth() {
        int total = 0;
        for (int i = 0; i < phases.Count; i++) {
            if (i != currentPhase || IsInvincible) {
                total += phases[i].currentHealth + phases[i].bonusHealth;
            }
        }
        return total;
    }

    int GetActiveHealth() {
        int total = 0;
        for (int i = 0; i < phases.Count; i++) {
            total += phases[i].currentHealth + phases[i].bonusHealth;
        }
        return total;
    }

    public float ActiveHealthFrac() {
        return GetActiveHealth() / (float)GetTotalMaxHealth();
    }
    public float ReserveHealthFrac() {
        return GetReserveHealth() / (float)GetTotalMaxHealth();
    }

    public bool TimerIsActive() {
        return !waitingOnPhaseChange;
    }


    protected void Update() {
        UpdateTime();
    }

    public void Init() {
        ResetHealth();
        PreparePhase(0);

    }

    public void ResetHealth() {
        foreach (var phase in phases) {
            phase.currentHealth = phase.maxHealth;            
        }
        if (phases.Count > 0)
            phases[phases.Count - 1].bonusHealth = 0;
    }

    private void UpdateTime() {
        if (waitingOnPhaseChange)
            return;
        if (GameManager.Instance.gameIsOver)
            return;
        if (timeLeft > 0) {
            timeLeft = Mathf.Max(timeLeft - Time.deltaTime, 0);
        } else {
            PhaseTimeout();
        }
    }

    public void TakeDamage(int damageAmount) {

        if (IsInvincible) {
            if (invincibleHitEffect != null) {
                Instantiate(invincibleHitEffect, transform.position, transform.rotation, null);
            }
            return;
        }
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, transform.rotation, null);
        }

        if (currentPhase >= 0 && currentPhase < phases.Count) {
            phases[currentPhase].currentHealth -= damageAmount;
            if (phases[currentPhase].currentHealth <= 0) {
                PhaseBeaten();
            }
        }
    }

    private int GetPhaseScore(bool complete) {
        if (complete)
            return phases[currentPhase].phaseScore;

        float damageFrac = 1f - (Mathf.Max(phases[currentPhase].currentHealth, 0f) / (float)phases[currentPhase].maxHealth);        
        return (int)(phases[currentPhase].phaseScore * damageFrac);
    }
    private int GetTimeBonus() {
        float timeFrac = (Mathf.Min(timeLeft, phases[currentPhase].maxExtraTimeBonus) / (float)phases[currentPhase].maxExtraTimeBonus);
        return (int)(phases[currentPhase].phaseTimeBonus * timeFrac);
    }

    public void PhaseBeaten() {
        waitingOnPhaseChange = true;
        onPhaseBeaten.Invoke(currentPhase);
        AddToScore(GetPhaseScore(true), false);
        AddToScore(GetTimeBonus(), true);
    }

    public void PhaseTimeout() {
        waitingOnPhaseChange = true;
        onPhaseTimeout.Invoke(currentPhase);
        AddToScore(GetPhaseScore(false), false);
        if (phases.Count > 0) {
            phases[phases.Count - 1].bonusHealth += phases[currentPhase].currentHealth;
            phases[currentPhase].currentHealth = 0;
        }
    }

    public void PreparePhase(int newPhase) {
        waitingOnPhaseChange = true;

        currentPhase = newPhase;
        phases[newPhase].currentHealth = phases[newPhase].maxHealth + phases[newPhase].bonusHealth;
        phases[newPhase].bonusHealth = 0;
        timeLeft = phases[newPhase].phaseTime + (phases[newPhase].extraTimeCarriesOver ? Mathf.Max(timeLeft, 0) : 0);
        onPhasePreStart.Invoke(newPhase);
    }

    public void StartPhase() {
        waitingOnPhaseChange = false;
        onPhaseStart.Invoke(currentPhase);

    }

    public void Remove() {
        if (_isEnding) // Prevent multiple calls to die
            return;
        _isEnding = true;
        _onRemove.Invoke(gameObject);
        Destroy(this.gameObject);
        _isEnding = false;

    }

    public void Die()
    {
        if (_isEnding) // Prevent multiple calls to die
            return;
        _isEnding = true;
        _onDeath.Invoke(gameObject);

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation, null);
        }

        // if (gameObject.GetComponent<Enemy>() != null)
        // {
        //     gameObject.GetComponent<Enemy>().DoBeforeDestroy();
        // }

        Destroy(this.gameObject);
        _isEnding = false;
    }

    private void AddToScore(int scoreValue, bool isTimeBonus)
    {
        if (GameManager.Instance != null && !GameManager.Instance.gameIsOver)
        {
            GameManager.Instance.AddScore(scoreValue, isTimeBonus);
        }
    }

}
