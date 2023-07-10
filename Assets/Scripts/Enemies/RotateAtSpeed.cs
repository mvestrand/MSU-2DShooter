using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAtSpeed : MonoBehaviour
{
    private float rotation;
    public float rotationSpeed = 0;

    void OnEnable() {
        rotation = transform.localEulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        rotation = Mod(rotation + rotationSpeed * Time.deltaTime, 360);
        transform.localEulerAngles = new Vector3(0,0, rotation);
    }

    private float Mod(float lhs, float rhs) {
        return (lhs % rhs + rhs) % rhs;
    }
}
