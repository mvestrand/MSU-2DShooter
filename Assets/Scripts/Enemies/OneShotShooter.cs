using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

using AYellowpaper;

public class OneShotShooter : MonoBehaviour
{
    [SerializeField] OptionalFloat lifetime;
    [SerializeField] List<InterfaceReference<IShootingController, MonoBehaviour>> shooters = new List<InterfaceReference<IShootingController, MonoBehaviour>>();

    void OnEnable() {
        foreach (var shooter in shooters) {
            shooter.Value.PlayOneShot();
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
            float dummy = 0;
            isDone = isDone && !shooter.Value.UpdateFireState(shouldFire, ref dummy);
        }
        // if (isDone)
        //     Pool.Release(gameObject);
    }
}
