using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Scriptables/Variables/Int", order = 2)]
public class IntVariable : ScriptableVariable<int> {}

[Serializable]
public class IntReference : ScriptableReference<IntVariable, int> {}

}
