using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPoolableObject {
    GameObject CreatePooledItem();
    void OnTakeFromPool(GameObject obj);
    void OnReturnedToPool(GameObject obj);
    void OnDestroyPoolObject(GameObject obj);
}

// public class Test : IPoolableObject<Test>
// {
//     public Test CreatePooledItem()
//     {
//         throw new System.NotImplementedException();
//     }

//     public void OnDestroyPoolObject(Test obj)
//     {
//         throw new System.NotImplementedException();
//     }

//     public void OnReturnedToPool(Test obj)
//     {
//         throw new System.NotImplementedException();
//     }

//     public void OnTakeFromPool(Test obj)
//     {
//         throw new System.NotImplementedException();
//     }
// }

// [System.Serializable]
// public class PooledPrefabReference<T> where T : IPoolableObject<T> {
//     public T Prefab;
//     // private ObjectPool;
//     public T GetObject() {


//     }
// }


