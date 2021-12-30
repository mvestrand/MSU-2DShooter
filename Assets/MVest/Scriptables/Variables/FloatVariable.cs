using System;
using UnityEngine;


namespace MVest {


[CreateAssetMenu(menuName="Scriptables/Variables/Float", order = 3)]
public class FloatVariable : ScriptableVariable<float> {}

[Serializable]
public class FloatReference : ScriptableReference<FloatVariable, float> {}

}
