using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrackPlayer : MonoBehaviour
{
    // public enum OutOfArcBehaviour {
    //     Idle,
    //     ReturnToDefault,
    //     TrackClosest
    // }

    // public enum NoTargetBehaviour {
    //     Idle,
    //     ReturnToDefault
    // }

    public bool trackingEnabled = true;
    // public OptionalFloat turnSpeed;
    // public OptionalFloat minAngleOffset;
    // public OptionalFloat maxAngleOffset;
    // public float defaultAngle;
    // public OptionalFloat range;


    // Update is called once per frame

    [SerializeField] bool invert = false;

    void Update()
    {
        float desiredRotation = 0;
        if (GameManager.Instance != null && GameManager.Instance.player != null) {

            var steerTarget = GameManager.Instance.player.transform;
            var targetDirection = new Vector3(steerTarget.position.x - transform.position.x, steerTarget.position.y - transform.position.y, 0).normalized;
            float angle = Vector3.SignedAngle((invert ? Vector3.down : Vector3.up), targetDirection, Vector3.forward);
            desiredRotation = angle;
        }



        transform.eulerAngles = new Vector3(0, 0, desiredRotation);
    }
}
