using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimatorTrackPlayer : MonoBehaviour
{
    public Animator animator;
    public string parameterName;

    [SerializeField] bool invert = false;

    void Update()
    {
        // float desiredRotation = 0;
        if (GameManager.Instance != null && GameManager.Instance.player != null) {

            var steerTarget = GameManager.Instance.player.transform;
            var targetDirection = new Vector3(steerTarget.position.x - transform.position.x, steerTarget.position.y - transform.position.y, 0).normalized;
            float angle = Vector3.SignedAngle((invert ? Vector3.down : Vector3.up), targetDirection, Vector3.forward);
            // desiredRotation = angle;
            angle = (angle % 360f + 360f) % 360f;

            animator.SetFloat(parameterName, angle / 360f);
        }



        // transform.eulerAngles = new Vector3(0, 0, desiredRotation);
    }
}
