using System;

using UnityEngine;

using MVest;

[DisallowMultipleComponent]
public sealed class SpawnController : MonoBehaviour
{
    private static int collisionLayer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void GetCollisionLayer() {
        collisionLayer = LayerMask.NameToLayer("SpawnObject");
        if (collisionLayer == -1) {
            Debug.LogError("To use spawn areas the collision layer \"SpawnObject\" must exist"); 
        }
    }



    [Tooltip("This determines how far outside the spawn box this object should be to not spawn on screen")]
    [SerializeField] float spawnRadius;

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
        if (collisionLayer == -1) {
            Debug.LogError("To use spawn areas the collision layer \"SpawnObjects\" must exist"); 
        } 
        else if (gameObject.layer != collisionLayer) {
            Debug.LogWarningFormat("Spawn controller {0} is not set to layer \"SpawnObject\". Setting to correct layer", this.FullName());
            gameObject.layer = collisionLayer;
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
