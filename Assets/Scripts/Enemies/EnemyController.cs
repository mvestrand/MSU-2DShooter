using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;

using UnityEngine.Timeline;

public class EnemyController : MonoBehaviour  {

    public Enemy prefab;
    [System.NonSerialized] public EnemyControlBehaviour behaviour;

    public void SpawnEnemy() {
            var startPoint = GetComponent<BezierSpline>().GetPoint(0);
            if (behaviour.instance == null)
                behaviour.instance = Instantiate(prefab, startPoint, Quaternion.identity);
    }

    public void FireOnce() {

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.localPosition, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.localPosition, transform.localPosition + transform.localRotation * Vector3.down);
    }

}
