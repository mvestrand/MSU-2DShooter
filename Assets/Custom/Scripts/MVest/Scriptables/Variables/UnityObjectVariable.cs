using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Scriptables/Variables/Unity Object", order = 1)]
public class UnityObjectVariable : ScriptableVariable<UnityEngine.Object> {}

[Serializable]
public class UnityObjectReference : ScriptableReference<UnityObjectVariable, UnityEngine.Object> {}

}
