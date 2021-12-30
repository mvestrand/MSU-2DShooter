using System;

using UnityEngine;

using MVest;

[DisallowMultipleComponent]
public sealed class SpawnController : MonoBehaviour
{
    private static int collisionLayer;

    static SpawnController() {
        collisionLayer = LayerMask.NameToLayer("SpawnObject");
        if (collisionLayer == -1) {
            Debug.LogError("To use spawn areas the collision layer \"SpawnObject\" must exist");
        }
    }

    [Tooltip("This determines how far outside the spawn box this object should be to not spawn on screen")]
    [SerializeField] Rect maxAABB;

    [Tooltip("Keep this object from despawning even when leaving the spawn area")]
    [SerializeField] bool _forbidDespawn = false;
    bool _hasLeftSpawnBox = false;
    int _intersectedSpawnAreas = 0;
    public event Action onDespawn;
    public bool ForbidDespawn { get { return _forbidDespawn; } }
    public bool HasLeftSpawnBox { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int IntersectedSpawnAreas { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void AllowDespawn() { _forbidDespawn = false; }

    public void DisallowDespawn() { _forbidDespawn = true; }

    public void Despawn() {
        onDespawn?.Invoke();
    }

    private void Awake() {
        ValidateSettings();
    }

    private void ValidateSettings() {
        if (gameObject.layer != collisionLayer) {
            if (collisionLayer != -1) {
                Debug.LogWarningFormat("Spawn controller {0} is not set to layer \"SpawnObject\". Setting to correct layer", this.GetExtendedName());
                gameObject.layer = collisionLayer;
            } else {
                Debug.LogWarningFormat("Spawn controller {0} is not set to layer \"SpawnObject\". The correct layer does not exist (try reloading assemblies)", this.GetExtendedName());
            }
        }
    }

    
    void OnTriggerEnter2D(Collider2D other) {
        _hasLeftSpawnBox = false;
        _intersectedSpawnAreas++;
    }

    void OnTriggerExit2D(Collider2D other) {
        _hasLeftSpawnBox = true;
        _intersectedSpawnAreas--;
        TryDespawn();
    }

    private void TryDespawn() {
        if (!_forbidDespawn && _hasLeftSpawnBox && _intersectedSpawnAreas == 0) {
            Despawn();
        }
    }


}
