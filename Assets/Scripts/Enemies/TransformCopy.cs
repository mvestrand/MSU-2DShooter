using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCopy : MonoBehaviour
{
    public enum CopyMode {
        Ignore,
        Fixed,
        Copy,
        OffsetCopy
    }

    public float rotationOffset;
    public Vector2 positionOffset;
    public CopyMode rotationMode;

    public CopyMode positionMode;

    public Transform target;

    // Update is called once per frame
    void Update() {
        if (target != null) {
            switch (rotationMode) {
            case CopyMode.Copy:
                transform.rotation = target.rotation;
                break;
            case CopyMode.OffsetCopy:
                transform.eulerAngles = target.eulerAngles + new Vector3(0,0,rotationOffset);
                break;
            case CopyMode.Fixed:
                transform.eulerAngles = new Vector3(0, 0, rotationOffset);
                break;
            }

            switch (positionMode) {
            case CopyMode.Copy:
                transform.position = target.position;
                break;
            case CopyMode.OffsetCopy:
                transform.position = target.position + new Vector3(positionOffset.x, positionOffset.y, 0);
                break;
            case CopyMode.Fixed:
                transform.position = new Vector3(positionOffset.x, positionOffset.y, 0);
                break;
            }
        }
    }

}
