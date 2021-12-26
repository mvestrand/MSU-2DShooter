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

    public static void SetActiveInHierarchy(this GameObject obj) {
        obj.SetActive(true);
        Transform parent = obj.transform.parent;
        while (parent != null) {
            parent.gameObject.SetActive(true);
            parent = parent.transform.parent;
        }
    }
}

}

