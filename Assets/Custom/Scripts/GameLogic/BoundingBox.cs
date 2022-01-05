using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

public class BoundingBox : MonoBehaviour
{
    public Rect bounds;
    //public BoundingBoxVariable boxVariable;
    public ObjectHookSetter<BoundingBox> objectHook;


    public bool Contains(in Vector3 pos) {
        return bounds.Contains(new Vector2(pos.x, pos.y));
    }

    public Vector3 ClampXY(Vector3 pos) {
        pos.x = Mathf.Clamp(pos.x, bounds.xMin, bounds.xMax);
        pos.y = Mathf.Clamp(pos.y, bounds.yMin, bounds.yMax);
        return pos;
    }

    void OnEnable() {
        objectHook.Attach(this);
    }

    void OnDisable() {
        objectHook.Detach(this);
    }

    void OnDrawGizmos() {
        Gizmos.color = objectHook.Color * Color.gray;
        DrawRect(ref bounds);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = objectHook.Color * new Color(1.5f, 1.5f, 1.5f, 1f);
        DrawRect(ref bounds);
    }

    private void DrawRect(ref Rect rect) {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

}
