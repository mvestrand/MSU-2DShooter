using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

using AYellowpaper;

[System.Serializable]
public class BossGunGroup {

    public List<InterfaceReference<IShootingController, MonoBehaviour>> guns = new List<InterfaceReference<IShootingController, MonoBehaviour>>();
}

[SelectionBase]
public class Boss : MonoBehaviour
{

    public List<BossGunGroup> gunGroups = new List<BossGunGroup>();

    [SerializeField] private PlayableDirector director;
    [SerializeField] private List<double> destinationTimes = new List<double>();

    public int gunGroup;

    public bool shouldShoot = false;

    protected void LateUpdate() {
        TryToShoot();
    }

    private void TryToShoot()
    {
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
        director.time = destTime;
    }

    public void onPhaseBeaten(int phase) {
        GetComponent<SeekTarget>().LockPosition();
        JumpToTime(phase);
    }

}
