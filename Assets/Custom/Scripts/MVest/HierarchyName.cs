using UnityEngine;

namespace MVest{

public static class GameObject_Extensions_Hierarchy
{
    public static string HierarchyName(this GameObject obj) {
        if (obj == null)
                return "Null";

        GameObject curObject = obj;
        string path = obj.name;
        while (curObject.transform.parent != null) {
            curObject = curObject.transform.parent.gameObject;
            path = curObject.name + "/" + path;
        }
        return path;
    }
}

}

