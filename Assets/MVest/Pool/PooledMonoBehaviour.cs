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
    [FoldoutGroup("Pooling Settings")]
    private PooledMonoBehaviour _prototype;

    public int defaultPoolCapacity() {
        return _defaultPoolCapacity;
    }

    public int GetPrefabID() {
        if (_prototype != null)
            return _prototype.gameObject.GetInstanceID();
        return this.gameObject.GetInstanceID();
    }

    public string GetPrefabName() {
        if (_prototype != null)
            return _prototype.gameObject.name;
        return this.gameObject.name;            
    }

    public string GetPrefabPath() {
        if (_prototype != null)
            return _prototype._path;
        return this._path;            
    }

    public int maxPoolSize() {
        return _maxPoolSize;
    }


    public GameObject CreatePooledItem() {
        PooledMonoBehaviour obj;

        obj = Instantiate<PooledMonoBehaviour>(this);
        obj._prototype = this;

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
        obj.SetActive(true);
        comp.Restart();
    }


    private GameObject GetObject() {
        if (_pool == null) {
            if (GameObjectPoolManager.Instance == null) {
                return CreatePooledItem();
            }
            _pool = GameObjectPoolManager.Instance.GetPool(this);
        }
        GameObject obj = _pool.Get();
        if (GameObjectPoolManager.Instance.EnableReleaseTracing) _pool.ReportGet(obj);
        return obj;
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
        if (this != null) {
            if (_pool != null) {
                if (GameObjectPoolManager.Instance.EnableReleaseTracing) _pool.ReportRelease(this.gameObject, StackTraceUtility.ExtractStackTrace());
                _pool.Release(this.gameObject);
            } else {
                if (GameObjectPoolManager.Instance.EnableReleaseTracing) _pool.ReportDestroy(this.gameObject);
                Destroy(this.gameObject);
            }
        }
    }

    protected abstract void Restart();

    // TODO Rewrite existing pooled monobehavior scripts to use more clear language
    public virtual void TakeFromPool(PooledMonoBehaviour prototype) {}
}

}
