using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    public float rotation = 0;
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0,0,rotation);
    }
}
