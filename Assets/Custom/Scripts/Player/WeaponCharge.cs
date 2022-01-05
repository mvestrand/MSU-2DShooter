using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using MVest;

public class WeaponCharge : ChargedAbility {

    public GlobalInt weaponLevel;
    public GlobalFloat weaponCharge;
    public bool shouldResetWeaponLevel = false;
    public bool shouldResetWeaponCharge = false;
    [FoldoutGroup("Effects")] public EffectRef upgradeEffect;
    public List<Gun> weaponTiers;
    [Min(0)]
    public int curWeapon;

    protected override void OnPowerStart()
    {
        Debug.Log("Powerup");
        if (curWeapon >= weaponTiers.Count-1) // Do nothing when 
            return;

        base.OnPowerStart();
        this.ResetCharge();
        upgradeEffect.Play(transform);

        curWeapon = System.Math.Max(curWeapon, 0);
        if (curWeapon < weaponTiers.Count-1) {
            weaponTiers[curWeapon].gameObject.SetActive(false);
            curWeapon++;
            weaponTiers[curWeapon].gameObject.SetActive(true);
            
        }
    }

    public void Start() {
        if (!shouldResetWeaponCharge && weaponCharge != null) {
            this.ModifyCharge(weaponCharge.Value);
            if (weaponCharge != null)
                weaponCharge.Value = 0;
        }
        if (!shouldResetWeaponLevel && weaponLevel != null) {
            curWeapon = weaponLevel;
        } else {
            curWeapon = 0;
            if (weaponLevel != null)
                weaponLevel.Value = 0;
        }

        for (int i = 1; i < weaponTiers.Count; i++) {
            weaponTiers[i].gameObject.SetActive(false);;
        }
    }

    public override void Update() {
        base.Update();
        if (weaponLevel != null)
            weaponLevel.Value = curWeapon;
        if (weaponCharge != null)
            weaponCharge.Value = this.CurrentCharge / this.maximumCharge;
    }
}

