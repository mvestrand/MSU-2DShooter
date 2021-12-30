using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using MVest;

[RequireComponent(typeof(BoxCollider2D))]
[DisallowMultipleComponent]
public sealed class SpawnArea : MonoBehaviour
{


    public ObjectHookRef<SpawnArea> hookAsset;
    [SerializeField] Rect bounds;


    private BoxCollider2D _collider2D;
    private BoxCollider2D _Collider2D {
        get {
            if (_collider2D == null)
                _collider2D = GetComponent<BoxCollider2D>();
            return _collider2D;
        }
    }

    private void Awake() {
        _collider2D = GetComponent<BoxCollider2D>();
        ValidateSettings();
    }

    private void ValidateSettings() {
        if (!_collider2D.isTrigger) {
            Debug.LogWarningFormat("Spawn area collider {0} is not set to trigger. Setting to trigger", this.GetExtendedName());
            _collider2D.isTrigger = true;
        }
        if (LayerMask.NameToLayer("SpawnArea") == -1) {
            Debug.LogError("To use spawn areas the collision layer \"SpawnArea\" must exist");
        } else if (gameObject.layer != LayerMask.NameToLayer("SpawnArea")) {
            Debug.LogWarningFormat("Spawn area layer {0} is not set to \"SpawnArea\". Setting to correct layer", this.GetExtendedName());
            gameObject.layer = LayerMask.NameToLayer("SpawnArea");
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue * Color.gray;
        GizmoExtras.DrawBounds(_Collider2D.bounds);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        GizmoExtras.DrawBounds(_Collider2D.bounds);
    }

    void OnEnable() {
        hookAsset.Attach(this);
    }

    void OnDisable() {
        hookAsset.Detach(this);
    }

    public Bounds GetBounds() {
        return _Collider2D.bounds;
    }


}
