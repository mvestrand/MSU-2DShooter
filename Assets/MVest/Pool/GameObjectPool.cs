using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

using Sirenix.OdinInspector;

namespace MVest {

public class GameObjectPool : SerializedMonoBehaviour {
    public int CountActive { get { return pool.CountActive; } }
    public int CountAll { get { return pool.CountAll; } }
    public int CountInactive { get { return pool.CountInactive; } }
    public void Clear() { pool.Clear(); }
    public void Dispose() { pool.Dispose(); }
    internal Transform inactiveObjectTransform;

    private Dictionary<GameObject, string> releaseObjectCalls = new Dictionary<GameObject, string>();

    public void ReportRelease(GameObject obj, string stackTrace) {
        if (releaseObjectCalls.TryGetValue(obj, out var oldStackTrace)) {
            Debug.LogErrorFormat("Duplicate Release() calls! \nOld call: {0}, \nNew call: {1}", oldStackTrace, stackTrace);
        } else {
            releaseObjectCalls.Add(obj, stackTrace);
        }
    }

    public void ReportGet(GameObject obj) {
        releaseObjectCalls.Remove(obj);
    }

    public void ReportDestroy(GameObject obj) {
        if (!releaseObjectCalls.Remove(obj)) {

            string hierarchyName = "";
            if (obj != null)
                hierarchyName = obj.HierarchyName();
            Debug.LogErrorFormat("Failed to remove object <{0}>: {1}", obj.GetInstanceID(), hierarchyName);
        }
    }

    public GameObject Get() { 
        GameObject obj = pool.Get();
        obj.transform.parent = this.transform;
        return obj;
    }

    private int _nextId = 0;
    public int GetNextId() {
        return _nextId++;
    }


    public void Release(GameObject element) { 
        pool.Release(element); 
    }

    internal ObjectPool<GameObject> pool;

}

}