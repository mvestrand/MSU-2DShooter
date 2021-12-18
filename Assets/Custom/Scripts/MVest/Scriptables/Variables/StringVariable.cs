using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Scriptables/Variables/String", order = 4)]
public class StringVariable : ScriptableVariable<string> {}

[Serializable]
public class StringReference : ScriptableReference<StringVariable, string> {}

}
