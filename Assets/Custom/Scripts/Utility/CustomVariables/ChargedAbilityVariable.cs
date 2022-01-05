using System;

using UnityEngine;

using MVest;

[CreateAssetMenu(menuName="Scriptables/Variables/Charged Ability Variable", order = 10)]
public class ChargedAbilityVariable : GlobalVariable<ChargedAbility> {
    [SerializeField] private Color _debugColor = Color.white;
    public Color DebugColor { get { return _debugColor; } }
}

[Serializable]
public class ChargedAbilityReference : GlobalRef<ChargedAbilityVariable, ChargedAbility> {}


