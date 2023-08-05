using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Globals;

[ExecuteAlways]
[SelectionBase]
public class ParallaxControl : MonoBehaviour
{
    public GameObject target;
    public BackgroundController controller;
    public bool ignoreScaling = false;

    //public float moveScale;
    protected void LateUpdate() {
        if (target != null && controller != null) {
            float t = Mathf.Max((controller.horizonDistance - transform.position.z) / controller.horizonDistance, 0);
            transform.localScale = (ignoreScaling ? Vector3.one : new Vector3(t, t, 1));
            target.transform.position = t * new Vector3(transform.position.x, transform.position.y, 0);
            if (target.TryGetComponent<SpriteRenderer>(out var renderer)) {
                renderer.sortingOrder = (int)(controller.sortingOrderScale * - transform.position.z);
            }
        }
    }

}
