using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    public Rect bounds;
    public BoundingBoxVariable boxVariable;



    public bool Contains(in Vector3 pos) {
        return bounds.Contains(new Vector2(pos.x, pos.y));
    }

    public Vector3 ClampXY(Vector3 pos) {
        pos.x = Mathf.Clamp(pos.x, bounds.xMin, bounds.xMax);
        pos.y = Mathf.Clamp(pos.y, bounds.yMin, bounds.yMax);
        return pos;
    }

    void OnEnable() {
        if(boxVariable != null)
            boxVariable.Value = this;
    }

    void OnDisable() {
        if (boxVariable != null && boxVariable.Value == this)
            boxVariable.Value = null;
    }

    void OnDrawGizmos() {
        if (boxVariable != null)
            Gizmos.color = (boxVariable.Value == this ? boxVariable.DebugColor : boxVariable.DebugColor * new Color(0.4f,.4f,.4f,1));
        else
            Gizmos.color = Color.gray;
        DrawRect(ref bounds);
    }

    void OnDrawGizmosSelected() {
        if (boxVariable != null) {
            Gizmos.color = (boxVariable.Value == this ? boxVariable.DebugColor : boxVariable.DebugColor * new Color(0.4f,.4f,.4f,1));
            Gizmos.color = Gizmos.color * new Color(1.5f, 1.5f, 1.5f, 1f);
        }
        else
            Gizmos.color = Color.white;
        DrawRect(ref bounds);
    }

    private void DrawRect(ref Rect rect) {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

}
