using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest {

public abstract class PooledMonoBehaviour : MonoBehaviour {

    [FoldoutGroup("Pooling Settings")]
    [Tooltip("The hierarchy path to store pooled objects in (the Pool Manager is the root)")]
    [SerializeField] private string _path;
    [FoldoutGroup("Pooling Settings")]
    [Tooltip("Minimum size of the pool including active objects")]
    [SerializeField] private int _minPoolSize = 0;
    [FoldoutGroup("Pooling Settings")]
    [Tooltip("Maximum size of the pool including active objects")]
    [SerializeField] private int _maxPoolSize = 10000;
    [ShowInInspector] internal GameObjectPool _pool;
    private PooledMonoBehaviour _original;
    public PooledMonoBehaviour Original { get { return (_original != null ? _original : this); }  }
    internal PooledMonoBehaviour nextPooledObject;
    internal PooledMonoBehaviour prevPooledObject;


    public GameObjectPool Pool { 
        get {
            if (_pool == null) {
                GetPool();
            }
            return _pool;
        }
    }

    private GameObjectPool GetPool() {
        if (_pool == null) {
                if (GameObjectPoolManager.Instance == null) {
                    return null; // We are not using pooling
                }
               _pool = GameObjectPoolManager.Instance.GetPool(this);
        }
        return _pool;
    }

    internal void _SetOriginal(PooledMonoBehaviour original) {
        _original = original;
    }

    public int MinPoolSize { get { return _minPoolSize; } }
    public int MaxPoolSize { get { return _maxPoolSize; } }

    public int PrefabID {
        get { return (_pool != null ? _pool.Original.gameObject.GetInstanceID() : this.gameObject.GetInstanceID()); }
    }

    public string PrefabName {
        get { return (_pool != null ? _pool.Original.gameObject.name : this.gameObject.name); }
    }

    public string PrefabPath {
        get { return (_pool != null ? _pool.Original._path : this._path); }
    }

    private PooledMonoBehaviour Get() {
        return (Pool != null ? Pool.Get() : Instantiate<PooledMonoBehaviour>(this));
    }


    public PooledMonoBehaviour Get(Transform parent) {
        return (Pool != null ? Pool.Get(parent) : Instantiate<PooledMonoBehaviour>(this, parent));
    }

    public PooledMonoBehaviour Get(Vector3 position, Quaternion rotation) {
        return (Pool != null ? Pool.Get(position, rotation) : Instantiate<PooledMonoBehaviour>(this, position, rotation));
    }

    public PooledMonoBehaviour Get(Vector3 position, Quaternion rotation, Transform parent) {
        return (Pool != null ? Pool.Get(position, rotation, parent) : Instantiate<PooledMonoBehaviour>(this, position, rotation, parent));
    }

    
    public T Get<T>()  where T : PooledMonoBehaviour {
        return (T)Get();
    }

    public T Get<T>(Transform parent)  where T : PooledMonoBehaviour {
        return (T)Get(parent);
    }

    public T Get<T>(Vector3 position, Quaternion rotation)  where T : PooledMonoBehaviour {
        return (T)Get(position, rotation);
    }

    public T Get<T>(Vector3 position, Quaternion rotation, Transform parent)  where T : PooledMonoBehaviour {
        return (T)Get(position, rotation, parent);
    }

    public void Release() {
        if (_pool != null) {
            if (GameObjectPoolManager.Instance.CollectionChecks) 
                _pool.CheckIntoPool(this, StackTraceUtility.ExtractStackTrace());
            _pool.Release(this);
        } else {
            Destroy(this.gameObject);
        }
    }


    public void RequestPreallocate(int number) {
        GetPool();
        this._pool?.RequestPreallocate(number);
    }

    public void CancelPreallocate(int number) {
        this._pool?.CancelPreallocate(number);
    }

    protected virtual void OnDestroy() {
        if (_pool != null)
            _pool.RemoveFromPool(this);

        // Guarantee that these are ACTUALLY set to null to prevent cyclic references
        nextPooledObject = null;
        prevPooledObject = null;
    }

    public abstract void Restart();
}

}
