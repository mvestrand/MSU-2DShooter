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
    [ShowInInspector] private GameObjectPool _pool;
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
        // if (_prefab == null) {
        obj = Instantiate<PooledMonoBehaviour>(this);
        obj._prefab = this;
        // } 
        // else { // This should never happen?
        //     obj = Instantiate<PooledMonoBehaviour>(_prefab);
        //     obj._prefab = _prefab;
        // }

        if (_pool != null) {
            obj.transform.parent = _pool.transform;
            obj._pool = _pool;
            obj.name = System.String.Format("{0} #{1:D4}", this.name, _pool.GetNextId());
        }
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
        obj.transform.SetPositionAndRotation(transform.localPosition, transform.localRotation);
        obj.transform.localScale = transform.localScale;

        PooledMonoBehaviour comp;
        obj.TryGetComponent<PooledMonoBehaviour>(out comp);
        comp.PreReactivate();
        obj.SetActive(true);
        comp.PreRestart();
    }


    private GameObject GetObject() {
        if (_pool == null) {
            if (GameObjectPoolManager.Instance == null) {
                return CreatePooledItem();
            }
            _pool = GameObjectPoolManager.Instance.GetPool(this);
        }
        return _pool.Get();
    }

    public GameObject Get() {
        GameObject obj = GetObject();
        PooledMonoBehaviour comp;
        obj.TryGetComponent<PooledMonoBehaviour>(out comp);
        comp.Restart();
        return obj;
    }

    public GameObject Get(Transform parent) {
        GameObject obj = GetObject();
        obj.transform.parent = parent;
        PooledMonoBehaviour comp;
        obj.TryGetComponent<PooledMonoBehaviour>(out comp);
        comp.Restart();
        return obj;
    }

    public GameObject Get(Vector3 position, Quaternion rotation) {
        GameObject obj = GetObject();
        obj.transform.SetPositionAndRotation(position, rotation);
        PooledMonoBehaviour comp;
        obj.TryGetComponent<PooledMonoBehaviour>(out comp);
        comp.Restart();
        return obj;
    }

    public GameObject Get(Vector3 position, Quaternion rotation, Transform parent) {
        GameObject obj = GetObject();
        obj.transform.parent = parent;
        obj.transform.SetPositionAndRotation(position, rotation);
        PooledMonoBehaviour comp;
        obj.TryGetComponent<PooledMonoBehaviour>(out comp);
        comp.Restart();
        return obj;
    }

    public T Get<T>() {
        GameObject  obj = Get();
        T comp;
        obj.TryGetComponent<T>(out comp);
        return comp;
    }

    public T Get<T>(Transform parent) {
        GameObject  obj = Get(parent);
        T comp;
        obj.TryGetComponent<T>(out comp);
        return comp;
    }

    public T Get<T>(Vector3 position, Quaternion rotation) {
        GameObject  obj = Get(position, rotation);
        T comp;
        obj.TryGetComponent<T>(out comp);
        return comp;
    }

    public T Get<T>(Vector3 position, Quaternion rotation, Transform parent) {
        GameObject  obj = Get(position, rotation, parent);
        T comp;
        obj.TryGetComponent<T>(out comp);
        return comp;
    }

    public void Release() {

        if (_pool != null) {
            _pool.Release(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }

    protected virtual void PreReactivate() {}
    protected virtual void PreRestart() {}
    protected abstract void Restart();
}

}
