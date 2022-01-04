using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Scriptables/Variables/Game Object", order = 6)]
public class GameObjectVariable : ScriptableVariable<UnityEngine.GameObject> {}

[Serializable]
public class GameObjectReference : ScriptableReference<GameObjectVariable, UnityEngine.GameObject> {}

}



