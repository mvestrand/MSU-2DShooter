using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Pool;

using System.Linq;

namespace MVest {

public sealed class GameObjectPoolManager : MonoBehaviour
{
    private bool _collectionChecks = true;
    private static bool _automaticallyInstantiate = true;
    private static bool _warnOnMissingPoolManager = true;
    public bool CollectionChecks { get { return _collectionChecks; } }

    private static GameObjectPoolManager _instance;
    public static GameObjectPoolManager Instance { 
        get { 
            if (_instance == null) {
                _instance = FindObjectOfType<GameObjectPoolManager>();

                if (_instance == null) {
                    if (_automaticallyInstantiate) {
                        _instance = new GameObject(nameof(GameObjectPoolManager)).AddComponent<GameObjectPoolManager>();
                        if (_warnOnMissingPoolManager)
                            Debug.LogWarning("This scene has no pool manager object. Creating it automatically.");
                    } 
                    else if (_warnOnMissingPoolManager) {
                        Debug.LogWarning("This scene has no GameObjectPoolManager and will not use MonoBehaviour pooling");
                    }
                }
            }

            return _instance; 
        }
    }

    private Dictionary<int, GameObjectPool> pools = new Dictionary<int, GameObjectPool>();

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

    public GameObjectPool GetPool(PooledMonoBehaviour prefabObj)  {
        GameObjectPool pool;
        if (pools.TryGetValue(prefabObj.PrefabID, out pool))
            return pool;
        string[] dirs = prefabObj.PrefabPath.Split('/')
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .ToArray();
        pool = CreatePoolRecursive(transform, prefabObj, dirs, 0);
        pools[prefabObj.PrefabID] = pool;
        return pool;
    }

    private GameObjectPool CreatePoolRecursive(Transform parent, PooledMonoBehaviour prefabObj, string[] dirs, int index) {
        if (index < dirs.Length) { // Recursively traverse the given dirs until the end is reached
            Transform child = parent.Find(dirs[index]);
            if (child == null) {
                child = new GameObject(dirs[index]).transform;
                child.parent = parent;
            }
            return CreatePoolRecursive(child, prefabObj, dirs, index + 1);
        } else {
            GameObjectPool pool = CreateGameObjectPool(parent, prefabObj);
            pools[prefabObj.PrefabID] = pool;
            return pool;
        }
    }

    private GameObjectPool CreateGameObjectPool(Transform parent, PooledMonoBehaviour prefabRef){
        GameObject poolGameObject = new GameObject(prefabRef.PrefabName);
        poolGameObject.transform.parent = parent;

        GameObjectPool poolComponent = poolGameObject.AddComponent<GameObjectPool>();
        poolComponent.CreateInactiveObjectHolder();
        poolComponent.SetOriginal(prefabRef);
        poolComponent.SetOptions(CollectionChecks);

        return poolComponent;
    }


}

public interface IPoolableObject
{
    GameObject CreatePooledItem();
    void OnTakeFromPool(GameObject obj);
    void OnReturnedToPool(GameObject obj);
    void OnDestroyPoolObject(GameObject obj);


    int DefaultPoolCapacity();
    int maxPoolSize();

    int GetPrefabID();
    string GetPrefabPath();
    string GetPrefabName();
}

}