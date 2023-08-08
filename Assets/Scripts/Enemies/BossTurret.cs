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

    public void DeactivateTurret() {
        if (turret != null) {
            turret.GetComponent<Animator>().SetTrigger("deactivate");
        }
    }
}
