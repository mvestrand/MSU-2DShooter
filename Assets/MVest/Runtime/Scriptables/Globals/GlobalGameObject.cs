using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Global/Game Object", order = 6)]
public class GlobalGameObject : GlobalVariable<UnityEngine.GameObject> {}

[Serializable]
public class GlobalGameObjectRef : GlobalRef<GlobalGameObject, UnityEngine.GameObject> {}

}



