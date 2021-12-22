using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour {

    public bool useLocalRotation = false;

    public Vector3 Position {
        get {
            return transform.position;
        }
    }

    public Quaternion Rotation {
        get {
            if (useLocalRotation)
                return transform.localRotation;
            else
                return transform.rotation;
        }
    }


    void OnDrawGizmos() {
        Vector3 forward, up;
        if (useLocalRotation) {
            forward = transform.position + transform.localRotation * Vector3.forward;
            up = transform.position + transform.localRotation * Vector3.up;
        }
        else {
            forward = transform.position + transform.TransformDirection(Vector3.forward);
            up = transform.position + transform.TransformDirection(Vector3.up);
        }   
        Gizmos.color = Color.white * new Color(0.5f,0.5f,0.5f,1);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.blue  * new Color(0.5f,0.5f,0.5f,1);
        Gizmos.DrawLine(transform.position, forward);
        Gizmos.color = Color.green * new Color(0.5f,0.5f,0.5f,1);
        Gizmos.DrawLine(transform.position, up);
    }

    void OnDrawGizmosSelected() {
        Vector3 forward, up;
        if (useLocalRotation) {
            forward = transform.position + transform.localRotation * Vector3.forward;
            up = transform.position + transform.localRotation * Vector3.up;
        }
        else {
            forward = transform.position + transform.TransformDirection(Vector3.forward);
            up = transform.position + transform.TransformDirection(Vector3.up);
        }   
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, forward);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, up);
    }

}
