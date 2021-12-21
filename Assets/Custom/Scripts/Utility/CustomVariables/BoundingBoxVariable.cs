using System;

using UnityEngine;

using MVest;

[CreateAssetMenu(menuName="Scriptables/Variables/Bounding Box", order = 10)]
public class BoundingBoxVariable : ScriptableVariable<BoundingBox> {
    [SerializeField] private Color _debugColor = Color.white;
    public Color DebugColor { get { return _debugColor; } }
}

[Serializable]
public class BoundingBoxReference : ScriptableReference<BoundingBoxVariable, BoundingBox> {}


