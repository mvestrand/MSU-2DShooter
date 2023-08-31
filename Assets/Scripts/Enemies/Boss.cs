using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

using AYellowpaper;
using System.Linq;

[System.Serializable]
public class BossGunGroup {

    public List<InterfaceReference<IShootingController, MonoBehaviour>> guns = new List<InterfaceReference<IShootingController, MonoBehaviour>>();
}

[System.Serializable]
public class TurretGroup {

    public List<BossTurret> turrets = new List<BossTurret>();

}


[SelectionBase]
public class Boss : MonoBehaviour {

    public Transform spawnedEnemiesRoot;
    public List<BossGunGroup> gunGroups = new List<BossGunGroup>();
    public List<TurretGroup> turretGroups = new List<TurretGroup>();

    [SerializeField] private PlayableDirector director;
    [SerializeField] private List<double> destinationTimes = new List<double>();

    public int gunGroup;
    public int turretGroup;

    public bool shouldShoot = false;

    protected void LateUpdate() {
        TryToShoot();
    }

    private void TryToShoot() {
        if (gunGroup < 0 || gunGroup >= gunGroups.Count)
            return;
        var guns = gunGroups[gunGroup].guns;
        foreach (var gun in guns) {
            float dummy = 0;
            gun.Value.UpdateFireState(shouldShoot, ref dummy);
        }
    }

    private void JumpToTime(int index) {
        if (index < 0 || index >= destinationTimes.Count) {
            return;
        }

        double destTime = destinationTimes[index];
        //director.playableGraph.GetRootPlayable(0).SetTime(destTime);
        director.Pause();
        director.time = destTime;
        director.Resume();
    }

    public void onPhaseBeaten(int phase) {
        GetComponent<SeekTarget>().LockPosition();
        ClearEnemies();
        JumpToTime(phase);
    }

    public void ActivateTurrets(int group) {
        turretGroup = group;
        foreach (var turret in turretGroups[group].turrets) {
            turret.ActivateTurret();
        }
    }

    public void StartTurrets() {
        foreach (var turret in turretGroups[turretGroup].turrets) {
            turret.StartTurret();
        }
    }

    public void StopTurrets() {
        foreach (var turret in turretGroups[turretGroup].turrets) {
            turret.StopTurret();
        }
    }

    public void DeactivateTurrets(int group) {
        foreach (var turret in turretGroups[group].turrets) {
            turret.DeactivateTurret();
        }
    }

    public void ClearEnemies() {
        if(spawnedEnemiesRoot != null) {
            var spawnedEnemies = spawnedEnemiesRoot.GetComponentsInChildren<Health>();
            foreach (var enemy in spawnedEnemies) {
                enemy.Die();
            }
        }
    }

}
