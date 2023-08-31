using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTurret : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject turret;

    public void ActivateTurret() {
        if (turret != null) {
            DeactivateTurret();
        }
        turret = Instantiate(prefab, transform);
    }

    public void StartTurret() {
        if (turret.TryGetComponent<Enemy>(out var enemy)) {
            enemy.shootMode = Enemy.ShootMode.SetByTrack;
            enemy.shouldShoot = true;
            enemy.StartShootingSequence();
        }
    }
    public void StopTurret() {
        if (turret.TryGetComponent<Enemy>(out var enemy)) {
            enemy.shootMode = Enemy.ShootMode.Never;
            enemy.shouldShoot = false;
            enemy.StopShootingSequence();
        }
    }

    public void DeactivateTurret() {
        if (turret != null) {
            turret.GetComponent<Animator>().SetTrigger("deactivate");
        }
    }
}
