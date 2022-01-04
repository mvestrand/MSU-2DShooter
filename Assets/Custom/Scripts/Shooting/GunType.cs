using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

using MVest;


[CreateAssetMenu(menuName ="Custom/Gun Type")]
public class GunType : ScriptableObject
{
    [FoldoutGroup("Effects", 1000)]
    [Tooltip("The effect to create when this fires")]
    public EffectRef fireEffect;


    [System.Serializable]
    public struct ProjectileData {
        [Tooltip("Which projectile to spawn")]
        public int prefabIndex;
        [Tooltip("Position offset to spawn at")]
        public Vector2 posOffset;
        [Tooltip("How much offset to give the projectile after rotating")]
        public Vector2 rotatedOffset;

        [Tooltip("How many degrees to offset the projectile")]
        [BoxGroup("Direction")]
        [HorizontalGroup("Direction/Box",0.5f)]
        [LabelText("Offset:")]
        [LabelWidth(50)]
        public float dirOffset;
        [BoxGroup("Direction")]
        [LabelWidth(50)]
        [Tooltip("The random offset from dir")][Min(0)]
        [HorizontalGroup("Direction/Box",0.5f)]
        [LabelText("Spread:")]
        public float dirSpread;

        [BoxGroup("Speed")]
        [LabelWidth(50)]
        [HorizontalGroup("Speed/Box",0.5f)]
        [LabelText("Offset:")]
        [Tooltip("How much extra speed to give the projectile")]
        public float speedOffset;
        [LabelWidth(50)]
        [HorizontalGroup("Speed/Box",0.5f)]
        [LabelText("Spread:")]
        [Tooltip("How much to randomly adjust projectile speed")]
        public float speedSpread;
    }

    [System.Serializable]
    public struct ProjectilePrefabData {
        [HorizontalGroup("Data", .5f)][HideLabel]
        [Tooltip("The projectile prefab to use")]
        public Projectile prefab;

        [HorizontalGroup("Data", .5f)]
        [Tooltip("How many instances of the prefab to preload")]
        [LabelText("Preload:")]
        [LabelWidth(50)]
        public int preloadCount;
    }

    [Tooltip("The projectile types to be fired.")]
    public List<ProjectilePrefabData> shotPrefabs = new List<ProjectilePrefabData>();
    [Tooltip("The projectiles to be fired.")]
    public List<ProjectileData> shots = new List<ProjectileData>();

    [Tooltip("The minimum time between projectiles being fired (sec)")]
    [Min(0.001f)]
    public float fireRate = 0.05f;

    [Tooltip("Should the ")]
    public bool useSimplifiedUpdate = true;
    public bool fireOnTrigger = false;
    public DefaultTo<float, Const.Zero> warmupTime;
    public DefaultTo<float, Const.Zero> cooldownTime;

    public DefaultTo<int, Const.One> minBurstSize;
    public DefaultTo<int, Const.MaxValue> maxBurstSize;

    public void SpawnProjectiles(Transform transform, int teamId) {
        foreach (var shot in shots) {
            float z = Random.Range(-shot.dirSpread, shot.dirSpread) + shot.dirOffset;
            Vector3 pos = transform.TransformDirection((Vector3)shot.posOffset);


            Projectile shotObj = shotPrefabs[shot.prefabIndex].prefab.Get<Projectile>(transform.position, transform.rotation);
            shotObj.transform.position += pos;
            shotObj.transform.eulerAngles += new Vector3(0,0,z);
            shotObj.transform.position += shotObj.transform.TransformVector((Vector3)shot.rotatedOffset);
            shotObj.projectileSpeed += shot.speedOffset + Random.Range(-shot.speedSpread, shot.speedSpread);

            shotObj.TryGetComponent<Damage>(out var damage);
            damage.teamId = teamId;
        }

    }

    public void RequestPreallocate() {
        foreach (var prefab in shotPrefabs) {
            prefab.prefab.RequestPreallocate(prefab.preloadCount);
        }
        fireEffect.RequestPreallocate();

    }

    public void CancelPreallocate() {
        foreach (var prefab in shotPrefabs) {
            prefab.prefab.CancelPreallocate(prefab.preloadCount);
        }
        fireEffect.CancelPreallocate();

    }

}

[System.Flags]
public enum GunState
{
    Idle = 1,
    Warmup = 2,
    Firing = 4,
    Cooldown = 8
}

