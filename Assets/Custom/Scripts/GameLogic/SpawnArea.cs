using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using MVest;

[RequireComponent(typeof(BoxCollider2D))]
[DisallowMultipleComponent]
public sealed class SpawnArea : MonoBehaviour
{
    #region Public Interface
    public Bounds GetBounds() {
        return _Collider2D.bounds;
    }
    #endregion

 
    #region Unity Messages
    void Awake() {
        _collider2D = GetComponent<BoxCollider2D>();
        ValidateSettings();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue * Color.gray;
        GizmoExtras.DrawBounds(_Collider2D.bounds);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        GizmoExtras.DrawBounds(_Collider2D.bounds);
    }
    #endregion


    #region Private Static 
    private static int collisionLayer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void GetCollisionLayer() {
        collisionLayer = LayerMask.NameToLayer("SpawnArea");
        if (collisionLayer == -1) {
            Debug.LogError("To use spawn areas the collision layer \"SpawnArea\" must exist"); 
        }
    }
    #endregion

    #region Private Fields
    private BoxCollider2D _collider2D;
    private BoxCollider2D _Collider2D {
        get {
            if (_collider2D == null)
                _collider2D = GetComponent<BoxCollider2D>();
            return _collider2D;
        }
    }
    #endregion


    private void ValidateSettings() {
        if (collisionLayer == -1) {
            Debug.LogError("To use spawn areas the collision layer \"SpawnArea\" must exist");
        } else if (gameObject.layer != collisionLayer) {
            Debug.LogWarningFormat("Spawn area layer {0} is not set to \"SpawnArea\". Setting to correct layer", this.FullName());
            gameObject.layer = collisionLayer;
        }
    }

}
