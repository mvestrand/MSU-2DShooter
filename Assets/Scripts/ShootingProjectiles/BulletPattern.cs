using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

[System.Serializable]
public class ProjectileData {
    public Projectile prefab;
    public Vector2 offset;
    public FloatCurve projectileDirection = new FloatCurve();
    public FloatCurve rotation = new FloatCurve();
    public bool rotationAffectsProjectile = true;
    public Vector2 rotatedOffset;
    public FloatCurve speed = new FloatCurve();
}

[CreateAssetMenu(menuName = "Bullet Pattern")]
public class BulletPattern : ScriptableObject {

    [SerializeField] private List<ProjectileData> projectiles = new List<ProjectileData>();


    public void Spawn(Vector3 position, Quaternion rotation, Transform parent) {
        foreach (var projectile in projectiles) {
            if (projectile == null || projectile.prefab == null)
                continue;

            float rotationRand = (projectile.rotation.UsesCurve ? Random.Range(0f, 1f) : 0);
            Quaternion localRotation = Quaternion.AngleAxis(projectile.rotation.Get(rotationRand, 0), Vector3.forward);
            float projectileDirectionRand = (projectile.projectileDirection.UsesCurve ? Random.Range(0f, 1f) : 0);
            Quaternion projectileDirection = Quaternion.AngleAxis(projectile.projectileDirection.Get(projectileDirectionRand, 0), Vector3.forward);

            float speedRand = (projectile.speed.UsesCurve ? Random.Range(0f, 1f) : 0);


            Vector3 pos = position + rotation * new Vector3(projectile.offset.x, projectile.offset.y, 0);
            pos = pos + localRotation * rotation * new Vector3(projectile.rotatedOffset.x, projectile.rotatedOffset.y, 0);

            Quaternion rot = projectileDirection * (projectile.rotationAffectsProjectile ? localRotation : Quaternion.identity) * rotation;

            var instance = Pool.Instantiate(projectile.prefab.gameObject, pos, rot, parent).GetComponent<Projectile>();
            if (projectile.speed.Enabled) {
                instance.projectileSpeed = projectile.speed.Get(speedRand, projectile.prefab.projectileSpeed);
            }
        }
    }

    public void Spawn(Vector2 position, float rotation, Transform parent) {
        Spawn(new Vector3(position.x, position.y, 0), Quaternion.AngleAxis(rotation, Vector3.forward), parent);
    }
}
