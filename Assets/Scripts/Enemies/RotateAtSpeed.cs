using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAtSpeed : MonoBehaviour
{
    private float rotation;
    public float rotationSpeed = 0;
    public FloatCurve curve;
    public bool useCurve = false;

    void OnEnable() {
        rotation = transform.localEulerAngles.z;
        if (useCurve)
            rotationSpeed = curve.Get((curve.UsesT ? Random.Range(0f, 1f) : 0), 0);
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
