using UnityEngine;

namespace MVest{

public static class GameObjectExtensions
{
    public static string HierarchyName(this GameObject obj) {
        if (obj == null)
                return "null";

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

    // TODO fix PooledMonoBehaviour time of release. It should actually
    // be waiting till the end of the frame to release it.
    public static void DestroyPooled(this GameObject obj) {
        if (obj.TryGetComponent<PooledMonoBehaviour>(out var pooled)) {
            pooled.Release();
        } else {
            GameObject.Destroy(obj);
        }
    }

    // TODO Implement this to actually work correctly
    public static void DestroyPooled(this GameObject obj, float t) {
        if (obj.TryGetComponent<PooledMonoBehaviour>(out var pooled)) {            
            //pooled.Release(t);
        } else {
            GameObject.Destroy(obj,t);
        }
        throw new System.NotImplementedException();
    }
}

}

