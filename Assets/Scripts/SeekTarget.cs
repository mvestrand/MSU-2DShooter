using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekTarget : MonoBehaviour {
    public Vector3 velocity;
    public Transform target;

    public Vector3 position;

    public bool isLocked = false;
    public bool isActive = false;


    public const float Epsilon = 0.0001f;

    public float slowingRadius = 1;
    public float maxSpeed = 10;

    public float steerStrength = 100;

    // Update is called once per frame
    void FixedUpdate() {
        if (!isActive)
            return;
        transform.position = position;

        if (isLocked)
            return;

        var desiredVelocity = target.position - transform.position;
        var distance = desiredVelocity.magnitude;

        if (distance < slowingRadius) {
            desiredVelocity = desiredVelocity.normalized * maxSpeed * (distance / slowingRadius);
        } else {
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
        }

        var steering = desiredVelocity - velocity;

        velocity = Truncate(velocity + steering * Mathf.Clamp01(steerStrength * Time.fixedDeltaTime), maxSpeed);
        transform.position += velocity * Time.fixedDeltaTime;

        position = transform.position;

    }

    void LateUpdate() {
        if (!isActive)
            return;
        transform.position = position;
        var desiredVelocity = target.position - transform.position;
        var distance = desiredVelocity.magnitude;

        if (distance < Epsilon) {
            transform.position = target.position;
            position = transform.position;
            isActive = false;
        }

    }

    public void LockPosition() {
        position = transform.position;
        velocity = Vector3.zero;
        isLocked = true;
        isActive = true;
    }

    public void Release() {
        isLocked = false;
    }

    public void SetInactive() {
        isActive = false;
        isLocked = false;
        position = transform.position;
        velocity = Vector3.zero;
    }

    Vector3 Truncate(Vector3 v, float max) {
        float magnitude = v.magnitude;
        if (magnitude > max) {
            var i = Mathf.Clamp01(max / magnitude);
            return v * i;
        } else
            return v;
    }
}
