using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace MVest {


[CreateAssetMenu(menuName="Scriptables/Variables/Float", order = 3)]
public class FloatVariable : ScriptableVariable<float> {}

[Serializable]
public class FloatReference : ScriptableReference<FloatVariable, float> {}

}
