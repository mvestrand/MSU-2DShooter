using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {

public static class GizmoExtras
{

    public static void DrawRect(in Rect rect) {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0), new Vector3(rect.size.x, rect.size.y, 0));
    }

    public static void DrawBounds(in Bounds bounds) {
        // if (b)
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

}

}

