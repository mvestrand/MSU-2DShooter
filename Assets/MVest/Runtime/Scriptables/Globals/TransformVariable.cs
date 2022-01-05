using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Scriptables/Variables/Transform", order = 12)]
public class TransformVariable : GlobalVariable<Transform> {}

[Serializable]
public class TransformReference : GlobalRef<TransformVariable, Transform> {}

}
