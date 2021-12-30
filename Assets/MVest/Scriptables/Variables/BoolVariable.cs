using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Scriptables/Variables/Bool", order = 5)]
public class BoolVariable : ScriptableVariable<bool> {}

[Serializable]
public class BoolReference : ScriptableReference<BoolVariable, bool> {}

}
