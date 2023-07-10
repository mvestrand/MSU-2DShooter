using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

public class OneShotShooter : MonoBehaviour
{
    [SerializeField] OptionalFloat lifetime;
    [SerializeField] List<ShootingController> shooters;

    void OnEnable() {
        foreach (var shooter in shooters) {
            shooter.PlayOneShot();
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        if (lifetime.enabled) {
            lifetime.value -= Time.deltaTime;
        }
        bool shouldFire = lifetime.enabled && lifetime.value > 0;
        bool isDone = true;
        foreach (var shooter in shooters) {
            isDone = isDone && !shooter.UpdateFireState(shouldFire);
        }
        // if (isDone)
        //     Pool.Release(gameObject);
    }
}
