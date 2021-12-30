using System;

using UnityEngine;

using MVest;

[CreateAssetMenu(menuName="Scriptables/Variables/Shield", order = 10)]
public class ShieldVariable : ScriptableVariable<Shield> {
    [SerializeField] private Color _debugColor = Color.white;
    public Color DebugColor { get { return _debugColor; } }
}

[Serializable]
public class ShieldReference : ScriptableReference<ShieldVariable, Shield> {}


