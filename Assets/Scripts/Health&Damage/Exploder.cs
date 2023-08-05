using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

public class Exploder : MonoBehaviour {

    public float lifetime;
    [SerializeField] bool explodeOnDeath = true;
    [SerializeField] bool explodeOnTimer = false;
    [SerializeField] bool inheritRotation = true;
    [SerializeField] bool rotationTrackPlayer = false;
    [SerializeField] FloatCurve rotationOffset = new FloatCurve();

    [SerializeField] GameObject explodeEffect;
    bool _hasExploded = false;


    public void OnEnable() {
        if (explodeOnDeath) {
            GetComponent<IHealth>()?.OnDeath.AddListener(OnDeath);
        }
        _hasExploded = false;
    }

    public void OnDisable() {
        if (explodeOnDeath) {
            GetComponent<IHealth>()?.OnDeath.RemoveListener(OnDeath);
        }
    }

    public void Update() {
        if (explodeOnTimer) {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0)
                Explode();
        }
    }

    
    public void Explode() {
        if (_hasExploded)
            return;
        _hasExploded = true;
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
