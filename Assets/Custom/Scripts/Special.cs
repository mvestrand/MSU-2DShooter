using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpecialAbility : MonoBehaviour, ISpecialAbility {

    [Header("Charge Settings")]
    [Tooltip("Charge needed to activate this ability")]
    public float maximumCharge = 100;
    [Tooltip("The current charge of the ability")]
    public float currentCharge = 0;
    [Tooltip("The charge of this ability at initialization")]
    public float initialCharge = 0;
    [Tooltip("The rate the ability discharges when active")]
    public float activeDischargeRate = 1f;


    [Header("Passive Charge Settings")]
    [Tooltip("Does the ability passively charge up over time")]
    public bool hasPassiveCharge = false;
    [Tooltip("The maximum value below which the ability passively charges")]
    public float passiveChargeMaximum = 0;
    [Tooltip("The passive charge rate of the ability (points / sec)")]
    public float passiveChargeRate = 0;
    [Tooltip("Delay after using the ability before passive recharging begins (sec)")]
    public float passiveChargeDelay = 0;

    [Header("Passive Discharge Settings")]
    [Tooltip("Does this ability passively discharge")]
    public bool hasPassiveDischarge = false;
    [Tooltip("Should this ability still passively discharge even when full")]
    public bool dischargesAfterFull = false;
    [Tooltip("Minimum value below which the ability discharges")]
    public float passiveDischargeMinimum = 0;
    [Tooltip("Rate at which the ability passively discharges (points / sec)")]
    public float passiveDischargeRate = 0;
    [Tooltip("Delay after receiving some charge before passive discharging begins (sec)")]
    public float passiveDischargeDelay = 0f;

    [Header("Runtime Debug")]
    [System.NonSerialized][ShowInInspector]
    [Tooltip("Is the ability currently full")]
    public bool isFull = false;
    [Tooltip("Is the ability currently activated")]
    [System.NonSerialized][ShowInInspector]
    public bool isActive = false;
    private float lastChargedTime;
    private float lastActivatedTime;

    public void ModifyCharge(float charge) {
        currentCharge += charge;
        if (charge > 0)
            lastChargedTime = Time.time;
        isFull = currentCharge >= maximumCharge;
        currentCharge = Mathf.Clamp(currentCharge, 0, maximumCharge);
    }

    public void Activate() {
        if (!isFull)
            return;
        isActive = true;
        OnPowerStart();
    }

    public void Deactivate() {
        if (!isActive)
            return;
        isActive = false;
        isFull = false;
        lastActivatedTime = Time.time;
        OnPowerEnd();
    }

    protected virtual void OnPowerStart() {}
    protected virtual void OnPowerActiveUpdate() {}
    protected virtual void OnPowerEnd() {}



    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update() {
        UpdateCharge();
    }

    void Initialize() {
        currentCharge = initialCharge;
    }


    void UpdateCharge() {
        if (isActive) {
            OnPowerActiveUpdate();
            currentCharge -= activeDischargeRate * Time.deltaTime;
            if (currentCharge <= 0) {
                currentCharge = 0;
                Deactivate();
            }
        }
        else if (hasPassiveCharge && 
            currentCharge < passiveChargeMaximum && 
            lastActivatedTime + passiveChargeDelay <= Time.time) 
        {
            currentCharge = Mathf.MoveTowards(currentCharge, passiveChargeMaximum, passiveChargeRate * Time.deltaTime);
            isFull = (currentCharge >= maximumCharge);
            currentCharge = Mathf.Clamp(currentCharge, 0, maximumCharge);
        } 
        else if (hasPassiveDischarge && 
            currentCharge > passiveDischargeMinimum && 
            lastChargedTime + passiveDischargeDelay <= Time.time &&
            (!isFull || dischargesAfterFull)) 
        {
            currentCharge = Mathf.MoveTowards(currentCharge, passiveDischargeMinimum, passiveDischargeRate * Time.deltaTime);
            isFull = (currentCharge >= maximumCharge);
            currentCharge = Mathf.Clamp(currentCharge, 0, maximumCharge);
        }
    }
}

public interface ISpecialAbility {
    void ModifyCharge(float charge);
    void Activate();
    void Deactivate();
}