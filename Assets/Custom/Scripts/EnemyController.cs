using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using MVest;

public class EnemyController : MonoBehaviour {

    public bool useLocalRotation = false;

    public const int Gun0 = 1, Gun1 = 2, Gun2 = 4, Gun3 = 8, Gun4 = 16, Gun5 = 32, Gun6=64, Gun7=128;
    public const int AllGuns = 255;


    [NotNull] public BoundingBoxVariable despawnBox;
    public bool canDespawn = false;

    public bool ShouldDespawn(Vector3 position) {
        return canDespawn
            && despawnBox.Value != null
            && !despawnBox.Value.Contains(position);
    }

    public int ShootChannels = AllGuns;

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
        if (canDespawn)
            Gizmos.DrawIcon(transform.position, "d_RotateTool", true, Color.red);
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
        if (canDespawn)
            Gizmos.DrawIcon(transform.position, "d_RotateTool", true, Color.red);
    }

}

[Flags][System.Serializable]
public enum GunChannel {
    Gun0 = 1,
    Gun1 = 2,
    Gun2 = 4,
    Gun3 = 8,
    Gun4 = 16,
    Gun5 = 32,
    Gun6 = 64,
    Gun7 = 128
}