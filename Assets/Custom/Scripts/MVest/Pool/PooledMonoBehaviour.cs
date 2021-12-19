using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest {

public abstract class PooledMonoBehaviour : MonoBehaviour, IPoolableObject {

    [FoldoutGroup("Pooling Settings")]
    [Tooltip("The hierarchy path to store pooled objects in (the Pool Manager is the root)")]
    [SerializeField] private string _path;
    [FoldoutGroup("Pooling Settings")]
    [SerializeField] protected int _maxPoolSize = 10000;
    [FoldoutGroup("Pooling Settings")]
    [SerializeField] protected int _defaultPoolCapacity = 10;
    private GameObjectPool _pool;
    private PooledMonoBehaviour _prefab;

    public int defaultPoolCapacity() {
        return _defaultPoolCapacity;
    }

    public int GetPrefabID() {
        if (_prefab != null)
            return _prefab.gameObject.GetInstanceID();
        return this.gameObject.GetInstanceID();
    }

    public string GetPrefabName() {
        if (_prefab != null)
            return _prefab.gameObject.name;
        return this.gameObject.name;            
    }

    public string GetPrefabPath() {
        if (_prefab != null)
            return _prefab._path;
        return this._path;            
    }

    public int maxPoolSize() {
        return _maxPoolSize;
    }


    public GameObject CreatePooledItem() {
        PooledMonoBehaviour obj;
        if (_prefab == null) {
            obj = Instantiate<PooledMonoBehaviour>(this);
            obj._prefab = this;
        } else {
            obj = Instantiate<PooledMonoBehaviour>(_prefab);
            obj._prefab = _prefab;
        }
        obj.transform.parent = _pool.transform;
        return obj.gameObject;
    }

    public void OnDestroyPoolObject(GameObject obj) {
        Destroy(obj);
    }

    public void OnReturnedToPool(GameObject obj) {
        obj.SetActive(false);
        obj.transform.parent = _pool.transform;
    }

    public void OnTakeFromPool(GameObject obj) {

        obj.transform.parent = _pool.transform;
        PooledMonoBehaviour comp;
        obj.TryGetComponent<PooledMonoBehaviour>(out comp);
        comp.Reset();
        obj.SetActive(true);
    }


    public GameObject Get() {
        if (_pool == null) {
            if (GameObjectPoolManager.Instance == null) {
                return CreatePooledItem();
            }
            _pool = GameObjectPoolManager.Instance.GetPool(this);
        }
        return _pool.Get();
    }

    public void Release() {
        if (_pool != null) {
            _pool.Release(this.gameObject);
        } else {
            Destroy(this);
        }
    }

    protected abstract void Reset();
}

}
