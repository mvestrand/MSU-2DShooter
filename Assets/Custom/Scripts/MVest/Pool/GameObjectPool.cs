using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MVest {

public class GameObjectPool : MonoBehaviour {
    public int CountActive { get { return pool.CountActive; } }
    public int CountAll { get { return pool.CountAll; } }
    public int CountInactive { get { return pool.CountInactive; } }
    public void Clear() { pool.Clear(); }
    public void Dispose() { pool.Dispose(); }
    public GameObject Get() { 
        GameObject obj = pool.Get();
        obj.transform.parent = this.transform;
        return obj;
    }

    public void Release(GameObject element) { pool.Release(element); }

    internal ObjectPool<GameObject> pool;

}

}