using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shield : MonoBehaviour
{
    [Tooltip("Whether or not the shield is enabled")]
    private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }
    [Tooltip("Current shield charge remaining")]
    public float currentCharge;
    [Tooltip("Maximum shield charge value")]
    public float maximumCharge;
    [Tooltip("Rate the shield recharges (points / sec)")]
    public float rechargeRate;
    [Tooltip("Time delay before shield begins recharging (sec)")]
    public float rechargeDelay;


    [Tooltip("Whether or not breaking requires a full recharge")]
    public bool breakRequiresFullRecharge;
    [Tooltip("Delay before recharging begins after being broken")]
    public float brokenRechargeDelay;

    [Tooltip("Effect created when the shield breaks")]
    public GameObject breakEffect;
    public UnityEvent onBreak;

    [Tooltip("Damage taken from incoming elemental damage")]
    [NotNull]
    public ElementModifier damageModifier;
    [Tooltip("Charge generated from incoming elemental damage")]
    [NotNull]
    public ElementModifier chargeModifier;

    [Tooltip("The special to charge using this shield")]
    public ISpecialAbility special;


    private bool destroyOnBreak = false;
    private bool isBroken;
    private float brokeAtTime;
    private float disabledAtTime;


    public void SetActive(bool active) {
        if (active)
            Enable();
        else
            Disable();
    }

    public void Enable() {
        if (!IsActive) {
            _isActive = true;
        }
    }

    public void Disable() {
        if (IsActive) {
            _isActive = false;
            disabledAtTime = Time.time;
        }
    }

    public void AbsorbDamage(Damage damage) {
        Debug.Assert(IsActive, "Shield AbsorbDamage() should not be called while inactive");

        special?.ModifyCharge(chargeModifier.Modify(damage.element) * damage.chargeAmount);
        currentCharge -= damageModifier.Modify(damage.element) * damage.damageAmount;
        if (currentCharge <= 0) {
            Break();
        }
        damage.NotifyAbsorb();
    }

    public void Break() {
        isBroken = true;
        brokeAtTime = Time.time;
        _isActive = false;
        currentCharge = 0;
        if (destroyOnBreak)
            Destroy(this.gameObject);
    }

    void Update() {
        UpdateBroken();
        UpdateCharge();
    }

    void UpdateCharge() {
        if (ShouldDischarge()) {

        }
        else if (ShouldRecharge()) {
            currentCharge = Mathf.Min(currentCharge + rechargeRate * Time.deltaTime, maximumCharge);
        }
    }

    void UpdateBroken() {
        if (isBroken) {
            if (brokeAtTime + brokenRechargeDelay <= Time.time && !breakRequiresFullRecharge) {
                isBroken = false;
            }
        }
    }

    bool ShouldDischarge() {
        return IsActive;
    }

    bool ShouldRecharge() {
        return (!isBroken && disabledAtTime + rechargeDelay <= Time.time) || brokeAtTime + brokenRechargeDelay <= Time.time;
    }

}
