using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using MVest;

public class Shield : MonoBehaviour, IDamagable
{
    #region Runtime Fields
    [FoldoutGroup("Runtime Data")]
    [Tooltip("Current shield charge remaining")]
    [ShowInInspector] private float _currentCharge;
    public float CurrentCharge { get { return _currentCharge; } }
    [FoldoutGroup("Runtime Data")]
    [Tooltip("Whether or not the shield is broken")]
    [FoldoutGroup("Runtime Data")]
    [ShowInInspector] private bool _isBroken;
    public bool IsBroken { get { return _isBroken; } }
    [Tooltip("Whether or not the shield is enabled")]
    [FoldoutGroup("Runtime Data")]
    [ShowInInspector] private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }
    [Tooltip("Whether or not the shield charge is full")]
    [FoldoutGroup("Runtime Data")]
    [ShowInInspector] private bool _isFull;
    public bool IsFull { get { return _isFull; } }
    private float _brokeAtTime;
    private float _disabledAtTime;
    private Animator _animator;
    #endregion

    #region Event Hooks
    [FoldoutGroup("Events")] public UnityEvent onBreak;
    [FoldoutGroup("Events")] public UnityEvent onActivate;
    [FoldoutGroup("Events")] public UnityEvent onDeactivate;
    [FoldoutGroup("Events")] public UnityEvent onReset;
    [FoldoutGroup("Events")] public UnityEvent onBreakClear;
    [FoldoutGroup("Events")] public UnityEvent onChargeFull;
    #endregion

    #region Configuration Fields
    [FoldoutGroup("Effects")]
    [Tooltip("Effect created when the shield breaks")]
    public GameObject breakEffect;

    [Tooltip("The team associated with this health")]
    [SerializeField] private int _teamId;
    public int TeamId { get { return _teamId; } }
    [Min(0)][Tooltip("The initial charge of the shield")]
    public float initialCharge = 0;
    public bool startActive = false;
    [Min(0)]
    [Tooltip("Minimum charge to activate the shield")]
    public float minimumActivateCharge = 10;
    [Min(0)]
    [Tooltip("Maximum shield charge value")]
    public float maximumCharge = 100;
    [Min(0)][Tooltip("Rate the shield recharges (points / sec)")]
    public float rechargeRate = 20;
    [Min(0)][Tooltip("Time delay before shield begins recharging (sec)")]
    public float rechargeDelay = .75f;

    [Min(0)][Tooltip("Charge used while active (points / sec)")]
    public float dischargeRate = 40;


    [Tooltip("Whether or not breaking requires a full recharge")]
    public bool breakRequiresFullRecharge = false;
    [Tooltip("Delay before recharging begins after being broken")]
    public float brokenRechargeDelay = 1.5f;


    [Tooltip("Damage taken from incoming elemental damage")]
    [NotNull]
    public ElementModifier damageModifier;
    public float damageModScale = 10f;
    [Tooltip("Charge generated from incoming elemental damage")]
    [NotNull]
    public ElementModifier chargeModifier;
    public float chargeModScale = 1;

    [Tooltip("The special to charge using this shield")]
    public ISpecialAbility special;
    [Tooltip("Should fully draining the shield count as breaking")]
    public bool breakOnFullDrain = true;
    [Tooltip("Should this component be disabled when deactivate is called")]
    public bool disableOnDeactivate = false;
    [Tooltip("Should this component be disabled when it is broken")]
    public bool disableOnBreak = false;
    #endregion

    #region Public Methods

    public bool CanActivate() {
        return !_isBroken && !_isActive && _currentCharge >= minimumActivateCharge;
    }


    [ContextMenu("Reset")]
    public void Reset() {
        _currentCharge = initialCharge;
        _disabledAtTime = Mathf.NegativeInfinity;
        _brokeAtTime = Mathf.NegativeInfinity;
        _isActive = false;
        _isBroken = false;
        onReset.Invoke();
    }

    [ContextMenu("Activate Shield")]
    public void Activate() {
        if (!IsActive) {
            _isActive = true;
            _animator.SetBool("Active", true);
            onActivate.Invoke();
        }
    }

    [ContextMenu("Deactivate Shield")]
    public void Deactivate() {
        if (IsActive) {
            _isActive = false;
            _disabledAtTime = Time.time;
            _animator.SetBool("Active", false);
            onDeactivate.Invoke();
        }
    }

    [ContextMenu("Break Shield")]
    public void Break() {
        _isBroken = true;
        _brokeAtTime = Time.time;
        _currentCharge = 0;
        onBreak.Invoke();
        Deactivate();
    }

    [ContextMenu("Clear Shield Break")]
    public void ClearBreak() {
        _isBroken = false;
        onBreakClear.Invoke();
    }

    [ContextMenu("Set Full Charge")]
    public void SetFull() {
        if (!_isFull) // Only invoke on first frame it is full
            onChargeFull.Invoke();
        _isFull = true;
        _currentCharge = maximumCharge;
        if (_isBroken && !breakRequiresFullRecharge) // Clear the break if broken
            ClearBreak();
    }

    public void SetActive(bool active) {
        if (active)
            Activate();
        else
            Deactivate();
    }

    public void HandleDamage(Damage damage) {
        Debug.Assert(IsActive, "Shield HandleDamage() should not be called while inactive");
        special?.ModifyCharge(chargeModifier.Modify(damage.element) * damage.chargeAmount * chargeModScale);
        _currentCharge -= damageModifier.Modify(damage.element) * damage.damageAmount * damageModScale;
        if (_currentCharge <= 0) {
            Break();
        }
        damage.NotifyAbsorb();
    }

    #endregion

    #region Unity Messages    

    void Start() {
        _animator = GetComponent<Animator>();
        _currentCharge = initialCharge;
        if(startActive)
            Activate();
        else
            Deactivate();
        _animator.Play("Entry");

    }

    void Update() {
        UpdateBrokenDelay();
        UpdateCharge();
    }

    #endregion

    #region Private Methods
    void UpdateCharge() {
        if (ShouldDischarge()) {
            _currentCharge -= dischargeRate * Time.deltaTime;
            if (_currentCharge <= 0) {
                if (breakOnFullDrain)
                    Break();
                else {
                    _currentCharge = 0;
                    Deactivate();
                }
            }
        } else if (ShouldRecharge()) {
            _currentCharge += rechargeRate * Time.deltaTime;
            if (_currentCharge >= maximumCharge) {
                SetFull();
            }
        }
    }


    void UpdateBrokenDelay() {
        if (_isBroken) {
            if (_brokeAtTime + brokenRechargeDelay <= Time.time && !breakRequiresFullRecharge) {
                ClearBreak();
            }
        }
    }

    bool ShouldDischarge() {
        return IsActive;
    }

    bool ShouldRecharge() {
        return (!_isBroken && _disabledAtTime + rechargeDelay <= Time.time) || _brokeAtTime + brokenRechargeDelay <= Time.time;
    }
    #endregion
}
