using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {

public interface ISimplePoolable {
    void Init();
    void Reset();
    void Release();
    void SetPool(GameObjectPool pool);
}

[System.Serializable]
public struct PooledReference<T> : IPoolableObject where T : MonoBehaviour, ISimplePoolable {
    [SerializeField] private string _path;
    [SerializeField] private T _prefab;
    private GameObjectPool _pool;
    public GameObject CreatePooledItem() {
        T obj = Object.Instantiate<T>(_prefab);
        obj.Init();
        obj.SetPool(_pool);
        return obj.gameObject;
    }

    public void OnTakeFromPool(GameObject obj) {
        obj.SetActive(true);
        T comp;
        if (obj.TryGetComponent<T>(out comp))
            comp.Reset();
    }
    public void OnReturnedToPool(GameObject obj) {
        obj.SetActive(false);
        obj.transform.parent = _pool.transform; 
    }
    public void OnDestroyPoolObject(GameObject obj) { Object.Destroy(obj); }

    public int defaultPoolCapacity() { return 10; }

    public string GetPrefabName() { return _prefab.name; }

    public string GetPrefabPath() { return _path; }

    public int maxPoolSize() { return 10000; }


    public T Get() {
        GameObject obj;
        T res;
        if (_prefab == null)
            return null;
        if (_pool == null) {
            if (GameObjectPoolManager.Instance == null) { // Don't use pooling
                obj = CreatePooledItem();
                obj.TryGetComponent<T>(out res);
                return res;
            }
            _pool = GameObjectPoolManager.Instance.GetPool<PooledReference<T>>(this);
        }
        obj = _pool.Get();
        obj.TryGetComponent<T>(out res);
        return res;
    }

    public int GetPrefabID() {
        return _prefab.GetInstanceID();
    }
}

}
