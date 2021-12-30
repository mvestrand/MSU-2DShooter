using System;

using UnityEngine;

using MVest;

[CreateAssetMenu(menuName="Scriptables/Variables/Spawn Box", order = 10)]
public class SpawnBoxVariable : ScriptableVariable<SpawnArea> {
    [SerializeField] private Color _debugColor = Color.white;
    public Color DebugColor { get { return _debugColor; } }
}

[Serializable]
public class SpawnBoxReference : ScriptableReference<SpawnBoxVariable, SpawnArea> {}


