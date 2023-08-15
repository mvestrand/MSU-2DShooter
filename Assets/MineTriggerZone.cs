using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MineTriggerZone : MonoBehaviour
{

    public UnityEvent onEnter = new UnityEvent();
    [SerializeField] int teamId = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IHealth collidedHealth = collision.gameObject.GetComponent<IHealth>();
        if (collidedHealth != null && collidedHealth.TeamId != this.teamId) {
            onEnter.Invoke();
        }
    }

}
