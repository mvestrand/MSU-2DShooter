using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using MVest;

[System.Serializable]
public class ChargedAbility : MonoBehaviour, IChargedAbility {

    public ChargedAbilityVariable variableHook;

    #region Runtime Fields
    [Header("Runtime Data")]
    [Tooltip("The current charge of the ability")]
    [ShowInInspector] private float _currentCharge = 0;
    [Tooltip("Is the ability currently full")]
    [ShowInInspector] private bool _isFull = false;
    public bool IsFull { get { return _isFull; } }
    [Tooltip("Is the ability currently activated")]
    [ShowInInspector] private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }
    private float _lastChargedTime;
    private float _lastActivatedTime;
    #endregion

    #region Configuration Fields
    [Header("Charge Settings")]
    [Min(1)][Tooltip("Charge needed to activate this ability")]
    public float maximumCharge = 100;
    [Tooltip("The charge of this ability at initialization")]
    public float initialCharge = 0;
    [Tooltip("The rate the ability discharges when active")]
    public float activeDischargeRate = 1f;
    [Tooltip("Whether or not activation requires having a full charge")]
    public bool activateRequiresFullCharge = true;
    [Tooltip("Minimum activation charge")]
    public float minimumActivationCharge = 1;

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
    #endregion

    #region Public Properties
    public float CurrentCharge { 
        get { return _currentCharge; } 
        set { 
            if (value == _currentCharge) // Return early if there is no change in the 0
                return;
            if (value > _currentCharge) {
                float deltaCharge = _currentCharge - value;
                _currentCharge = value;
                _lastChargedTime = Time.time;                
                UpdateFull();
                _currentCharge = Mathf.Min(_currentCharge, maximumCharge);
                onChargeGain.Invoke(deltaCharge);
            } else {
                float deltaCharge = _currentCharge - value;
                _currentCharge = value;
                UpdateFull();
                _currentCharge = Mathf.Max(_currentCharge, 0);
                onChargeLoss.Invoke(deltaCharge);
            }
        }
    }
    #endregion

    #region Event Hooks
    [FoldoutGroup("Events")] public UnityEvent onFull;
    [FoldoutGroup("Events")] public UnityEvent onLoseFull; // Called when ability is no longer full but has not been activated 
    [FoldoutGroup("Events")] public UnityEvent onActivate;
    [FoldoutGroup("Events")] public UnityEvent onDeactivate;
    [FoldoutGroup("Events")] public UnityFloatEvent onChargeGain;
    [FoldoutGroup("Events")] public UnityFloatEvent onChargeLoss;
    
    #endregion

    #region Public Methods
    public void ModifyCharge(float deltaCharge) {
        if (deltaCharge == 0) 
            return;
        _currentCharge += deltaCharge;
        UpdateFull();
    }

    public bool CanActivate() {
        return _isFull || (!activateRequiresFullCharge && _currentCharge >= minimumActivationCharge);
    }

    public void Activate() {
        if (!CanActivate())
            return;
        _isActive = true;
        _isFull = false;
        OnPowerStart();
    }

    public void Deactivate() {
        if (!_isActive)
            return;
        _isActive = false;
        _lastActivatedTime = Time.time;
        OnPowerEnd();
    }

    public virtual void Update() {
        UpdateCharge();
        if (_isActive)
            OnPowerActiveUpdate();
    }

    public void OnEnable() {
        if (variableHook != null && variableHook.Value == null) {
            variableHook.Value = this;
        }
    }

    public void OnDisable() {
        if (variableHook != null && variableHook.Value == this) {
            variableHook.Value = null;
        }
    }

    public void ResetCharge() {
        Deactivate();
        _currentCharge = 0;
    }

    public void Reset() {
        _currentCharge = initialCharge;
    }
    #endregion

    #region Subclass Override Methods
    protected virtual void OnPowerStart() {}
    protected virtual void OnPowerActiveUpdate() {}
    protected virtual void OnPowerEnd() {}
    #endregion

    #region Private Methods
    private void UpdateCharge() {
        if (_isActive) {
            OnPowerActiveUpdate();
            _currentCharge -= activeDischargeRate * Time.deltaTime;
            if (_currentCharge <= 0) {
                _currentCharge = 0;
                Deactivate();
            }
        }
        else if (hasPassiveCharge && 
            _currentCharge < passiveChargeMaximum && 
            _lastActivatedTime + passiveChargeDelay <= Time.time) 
        {
            _currentCharge = Mathf.MoveTowards(_currentCharge, passiveChargeMaximum, passiveChargeRate * Time.deltaTime);
            UpdateFull();
            _currentCharge = Mathf.Clamp(_currentCharge, 0, maximumCharge);
        } 
        else if (hasPassiveDischarge && 
            _currentCharge > passiveDischargeMinimum && 
            _lastChargedTime + passiveDischargeDelay <= Time.time &&
            (!_isFull || dischargesAfterFull)) 
        {
            _currentCharge = Mathf.MoveTowards(_currentCharge, passiveDischargeMinimum, passiveDischargeRate * Time.deltaTime);
            UpdateFull();
            _currentCharge = Mathf.Clamp(_currentCharge, 0, maximumCharge);
        }
    }

    private void UpdateFull() {
    if (_currentCharge >= maximumCharge) {
        if (!_isFull) {
            _isFull = true;
            onFull.Invoke(); // Only call onFull the first frame that it is set full
        }
    }
    else {
        if (_isFull) {
            _isFull = false;
            onLoseFull.Invoke();
        }
    }
    #endregion
}

}

public interface IChargedAbility {
    void ModifyCharge(float charge);
    void Activate();
    void Deactivate();
}