using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

/// <summary>
/// This class handles the dealing of damage to health components.
/// </summary>
public class Damage : MonoBehaviour
{
    [Header("Team Settings")]
    [Tooltip("The team associated with this damage")]
    public int teamId = 0;

    [Header("Damage Settings")]
    [Tooltip("How much damage to deal")]
    public float damageAmount = 1;
    [Tooltip("How much charge this damage generates")]
    public float chargeAmount = 1;
    [Tooltip("Elemental type of the damage (null is untyped)")]
    public Element element;
    [Tooltip("Prefab to spawn after doing damage")]
    public EffectRef hitEffect = new EffectRef();
    [Tooltip("Prefab to spawn when absorbed")]
    public EffectRef absorbEffect = new EffectRef();
    [Tooltip("Whether or not to destroy the attached game object after dealing damage")]
    public bool destroyAfterDamage = true;
    [Tooltip("Whether or not to destory the attached game object after being absorbed")]
    public bool destroyAfterAbsorb = true;
    [Tooltip("Whether or not to apply damage when triggers collide")]
    public bool dealDamageOnTriggerEnter = false;
    [Tooltip("Whether or not to apply damage when triggers stay, for damage over time")]
    public bool dealDamageOnTriggerStay = false;
    [Tooltip("Whether or not to apply damage on non-trigger collider collisions")]
    public bool dealDamageOnCollision = false;

    public bool ignoreShield = false;


    [Header("Graze Settings")]
    [Tooltip("Prefab to spawn after grazing the player")]
    public EffectRef grazeEffect = new EffectRef();
    [Tooltip("Whether or not this damage can trigger a graze when triggers collide")]
    public bool grazeOnTriggerEnter = true;
    [Tooltip("Whether or not this damage can trigger a graze when triggers stay, for multiple over time")]
    public bool grazeOnTriggerStay = false;
    [Tooltip("Cooldown after a graze occurs before it can occur again (sec)")]
    public float grazeCooldown = 1f;
    [Tooltip("Delay time after creation before the damage can cause a graze effect (sec)")]
    public float initialGrazeDelay = 0f;
    private static int GrazeCollisionLayer = 8;

    public IntReference pointsOnGraze = new IntReference();


    /// <summary>
    /// Description: 
    /// Standard Unity function called whenever a Collider2D enters any attached 2D trigger collider
    /// Inputs:
    /// Collider2D collision
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="collision">The Collider2D that set of the function call</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (grazeOnTriggerEnter && ShouldGraze(collision.gameObject)) {
            GrazeObject(collision.gameObject);
        }
        else if (dealDamageOnTriggerEnter)
        {
            DealDamage(collision.gameObject);
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called every frame a Collider2D stays in any attached 2D trigger collider
    /// Inputs:
    /// Collider2D collision
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="collision">The Collider2D that set of the function call</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (grazeOnTriggerStay && ShouldGraze(collision.gameObject)) {
            GrazeObject(collision.gameObject);
        }
        if (dealDamageOnTriggerStay)
        {
            DealDamage(collision.gameObject);
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called when a Collider2D hits another Collider2D (non-triggers)
    /// Inputs:
    /// Collision2D collision
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="collision">The Collision2D that set of the function call</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dealDamageOnCollision)
        {
            DealDamage(collision.gameObject);
        }
    }

    /// <summary>
    /// Description:
    /// This function deals damage to a health component if the collided 
    /// with gameobject has a health component attached AND it is on a different team.
    /// Inputs:
    /// GameObject collisionGameObject
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="collisionGameObject">The game object that has been collided with</param>
    private void DealDamage(GameObject collisionGameObject)
    {
        Shield collidedShield = collisionGameObject.GetComponent<Shield>();
        Health collidedHealth = collisionGameObject.GetComponent<Health>();
        if (collidedShield != null && collidedShield.IsActive && !this.ignoreShield)
        {
            if (collidedShield.TeamId != this.teamId)
            {
                collidedShield.HandleDamage(this);
            }
        } else if (collidedHealth != null)
        {
            if (collidedHealth.TeamId != this.teamId)
            {
                collidedHealth.HandleDamage(this);
            }
        } 
    }


    /// <summary>
    /// Notify this damage that it was a hit
    /// </summary>
    public void NotifyHit() {
        hitEffect.Fire(transform);
        if (destroyAfterDamage)
        {
            if (TryGetComponent<Enemy>(out var enemy))
            {
                enemy.DoBeforeDestroy();
            }
            if (TryGetComponent<Health>(out var health)) {
                health.Die();
            } else {
                gameObject.DestroyPooled();
            }
        }
    }

    /// <summary>
    /// Notify this damage that it was absorbed
    /// </summary>
    public void NotifyAbsorb() {
        absorbEffect.Fire(transform);
        // if (absorbEffect != null)
        // {
        //     absorbEffect.Fire(transform.position, transform.rotation);
        //     Instantiate(absorbEffect, transform.position, transform.rotation, null);
        // }
        if (destroyAfterAbsorb)
        {
            if (TryGetComponent<Enemy>(out var enemy))
            {
                enemy.DoBeforeDestroy();
            }
            if (TryGetComponent<Health>(out var health)) {
                health.Die();
            } else {
                gameObject.DestroyPooled();
            }
        }
    }



    private float lastGraze = System.Single.NegativeInfinity;

    /// <summary>
    /// Test if the object this is colliding with should cause a graze instead of damage
    /// </summary>
    /// <param name="collisionGameObject">The target game object</param>
    /// <returns>True if the target should cause a graze instead of damage</returns>
    private bool ShouldGraze(GameObject collisionGameObject) {
        return collisionGameObject.layer == GrazeCollisionLayer && lastGraze + grazeCooldown <= Time.time;
    }

    /// <summary>
    /// Graze against a player object
    /// </summary>
    /// <param name="collisionGameObject">The target object to graze against</param>
    private void GrazeObject(GameObject collisionGameObject) {
        GrazeArea target = collisionGameObject.GetComponent<GrazeArea>();
        if (target.Owner.TeamId == teamId) // Ignore objects on our team
            return;
        if (pointsOnGraze != null && pointsOnGraze > 0) {
            GameManager.AddScore(pointsOnGraze);
        }
        target.Owner.OnGraze();
        lastGraze = Time.time;
        grazeEffect.Fire(transform);
        // if (grazeEffect != null)
        // {
        //     Instantiate(grazeEffect, transform.position, transform.rotation, null);
        // }
    }


    private void OnEnable() {
        lastGraze = Time.time - grazeCooldown + initialGrazeDelay;
    }
}
