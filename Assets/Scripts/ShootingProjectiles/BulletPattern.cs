using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

[System.Serializable]
public class ProjectileData {
    [Tooltip("Projectile prefab")]
    public Projectile prefab;
    public ProjectileModifiers modifiers;
}

[CreateAssetMenu(menuName = "Bullet Pattern")]
public class BulletPattern : ScriptableObject {
    [SerializeField]private List<ProjectileData> projectiles = new List<ProjectileData>();

    public void Spawn(Vector3 position, Quaternion rotation, Transform parent, float advanceTime=0, Projectile fallback=null, ProjectileModifiers globalModifiers=null) {
        if (globalModifiers != null) {
            (position, rotation) = globalModifiers.ApplyOffsets(position, rotation);
        }
        
        foreach (var projectile in projectiles) {
            if (projectile == null)
                continue;

            if (projectile.prefab != null)
                projectile.prefab.Spawn(position, rotation, parent, advanceTime, projectile.modifiers);
            else
                fallback.Spawn(position, rotation, parent, advanceTime, projectile.modifiers);
        }
    }

    public void Spawn(Vector2 position, float rotation, Transform parent, float advanceTime=0, Projectile fallback=null, ProjectileModifiers globalModifiers=null) {
        Spawn(new Vector3(position.x, position.y, 0), Quaternion.AngleAxis(rotation, Vector3.forward), parent, advanceTime, fallback, globalModifiers);
    }
}
