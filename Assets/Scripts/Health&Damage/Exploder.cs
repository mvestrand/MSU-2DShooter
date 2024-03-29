using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

public class Exploder : MonoBehaviour {

    public float lifetime;
    public float currentLifetime;
    [SerializeField] bool explodeOnDeath = true;
    [SerializeField] bool explodeOnTimer = false;
    [SerializeField] bool inheritRotation = true;
    [SerializeField] bool rotationTrackPlayer = false;
    [SerializeField] float countdownTime = 1f;
    [SerializeField] GameObject countdownEffect;
    [SerializeField] FloatCurve rotationOffset = new FloatCurve();

    [SerializeField] GameObject explodeEffect;
    bool _hasExploded = false;
    [SerializeField] Animator animator;
    [SerializeField] string countdownTrigger = "countdown";

    GameObject countdownEffectInstance;

    public void OnEnable() {
        if (explodeOnDeath) {
            GetComponent<IHealth>()?.OnDeath.AddListener(OnDeath);
        }
        _hasExploded = false;
        countingDown = false;
        currentLifetime = lifetime;
        if (animator != null)
            animator.ResetTrigger(countdownTrigger);
    }

    public void OnDisable() {
        if (explodeOnDeath) {
            GetComponent<IHealth>()?.OnDeath.RemoveListener(OnDeath);
        }
    }
    bool countingDown = false;

    public void Update() {
        if (explodeOnTimer && !countingDown) {
            currentLifetime -= Time.deltaTime;
            if (currentLifetime <= 0) {
                if (countdownTime > 0) {
                    StartCountdown();
                } else {
                    Explode();
                }
            }
        } else if (countingDown) {
            currentLifetime -= Time.deltaTime;
            if (currentLifetime <= 0) {
                Explode();
            }
        }
    }


    public void StartCountdown() {
        if (countingDown)
            return;
        countingDown = true;
        currentLifetime = countdownTime;
        countdownEffectInstance = Pool.Instantiate(countdownEffect, transform.position, DetermineRotation(), transform);
        if (animator != null)
            animator.SetTrigger(countdownTrigger);
    }

    public void Explode() {
        if (_hasExploded)
            return;
        _hasExploded = true;
        if (countdownEffectInstance != null) {
            Pool.Release(countdownEffectInstance);
        }
        if (explodeOnDeath && TryGetComponent<IHealth>(out var health)) {
            health.Die();
        } else {
            SpawnExplosion();
            Pool.Release(this.gameObject);
        }
    }

    private void OnDeath(GameObject go) {
        SpawnExplosion();
    }

    private void SpawnExplosion() {
        Pool.Instantiate(explodeEffect, transform.position, DetermineRotation(), null);
    }

    private Quaternion DetermineRotation() {
        Quaternion rotation = (inheritRotation ? transform.rotation : Quaternion.identity);

        float t = (rotationOffset.UsesT ? Random.Range(0f, 1f) : 0);
        rotation = Quaternion.Euler(0, 0, rotationOffset.Get(t, 0)) * rotation;

        if (rotationTrackPlayer) {
            if (GameManager.Instance != null && GameManager.Instance.player != null) {

                var steerTarget = GameManager.Instance.player.transform;
                var targetDirection = new Vector3(steerTarget.position.x - transform.position.x, steerTarget.position.y - transform.position.y, 0).normalized;
                float angle = Vector3.SignedAngle(Vector3.up, targetDirection, Vector3.forward);
                rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        return rotation;
    }

}
