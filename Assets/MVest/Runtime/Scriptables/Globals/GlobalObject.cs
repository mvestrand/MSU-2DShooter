using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Global/Object", order = 1)]
public class GlobalObject : GlobalVariable<UnityEngine.Object> {}

[Serializable]
public class GlobalObjectRef : GlobalRef<GlobalObject, UnityEngine.Object> {}

}
