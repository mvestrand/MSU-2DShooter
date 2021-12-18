using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance { 
        get { 
            if (_instance == null) {
                _instance = FindObjectOfType<ObjectPoolManager>();
            }

            return _instance; 
        } 
    }


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

}
