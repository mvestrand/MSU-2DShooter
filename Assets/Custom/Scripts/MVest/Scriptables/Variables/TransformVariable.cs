using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Scriptables/Variables/Transform", order = 12)]
public class TransformVariable : ScriptableVariable<Transform> {}

[Serializable]
public class TransformReference : ScriptableReference<TransformVariable, Transform> {}

}
