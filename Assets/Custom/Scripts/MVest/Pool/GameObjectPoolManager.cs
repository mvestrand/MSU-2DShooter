using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace MVest {

public class GameObjectPoolManager : MonoBehaviour
{
    [SerializeField] private bool collectionChecks = true;

    private static GameObjectPoolManager _instance;
    public static GameObjectPoolManager Instance { 
        get { 
            if (_instance == null) {
                _instance = FindObjectOfType<GameObjectPoolManager>();
            }

            return _instance; 
        }
    }

    private Dictionary<int, GameObjectPool> pools;

    void Awake() {
        if (_instance == null) {
            _instance = this;
        }
        if (_instance != this) {
            Destroy(this);
        }
    }

    void OnDestroy() {
        if (_instance == this)
            _instance = null;
        
    }

    public GameObjectPool GetPool<T>(T prefabObj) where T : IPoolableObject {
        GameObjectPool pool;
        if (pools.TryGetValue(prefabObj.GetPrefabID(), out pool))
            return pool;
        string[] dirs = prefabObj.GetPrefabPath().Split('/')
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .ToArray();
        pool = CreatePoolRecursive<T>(transform, prefabObj, dirs, 0);
        pools[prefabObj.GetPrefabID()] = pool;
        return pool;
    }

    private GameObjectPool CreatePoolRecursive<T>(Transform parent, T prefabObj, string[] dirs, int index) where T : IPoolableObject {
        if (index < dirs.Length) { // Recursively traverse the given dirs until the end is reached
            Transform child = parent.Find(dirs[index]);
            if (child == null) {
                child = new GameObject(dirs[index]).transform;
                child.parent = parent;
            }
            return CreatePoolRecursive<T>(child, prefabObj, dirs, index + 1);
        } else {
            GameObjectPool pool = CreateGameObjectPool<T>(parent, prefabObj);
            pools[prefabObj.GetPrefabID()] = pool;
            return pool;
        }
    }

    private GameObjectPool CreateGameObjectPool<T>(Transform parent, T prefabRef) where T : IPoolableObject {
        GameObject poolGameObject = new GameObject(prefabRef.GetPrefabName());
        poolGameObject.transform.parent = parent;
        GameObjectPool poolComponent = poolGameObject.AddComponent<GameObjectPool>();
        poolComponent.pool = new UnityEngine.Pool.ObjectPool<GameObject>(
            prefabRef.CreatePooledItem, prefabRef.OnTakeFromPool, prefabRef.OnReturnedToPool, prefabRef.OnDestroyPoolObject, 
            collectionChecks, prefabRef.defaultPoolCapacity(), prefabRef.maxPoolSize());
        return poolComponent;
    }


}

public interface IPoolableObject
{
    GameObject CreatePooledItem();
    void OnTakeFromPool(GameObject obj);
    void OnReturnedToPool(GameObject obj);
    void OnDestroyPoolObject(GameObject obj);


    int defaultPoolCapacity();
    int maxPoolSize();

    int GetPrefabID();
    string GetPrefabPath();
    string GetPrefabName();
}

}