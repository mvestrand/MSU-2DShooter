using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.localPosition, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.localPosition, transform.localPosition + transform.localRotation * Vector3.down);
    }

}
