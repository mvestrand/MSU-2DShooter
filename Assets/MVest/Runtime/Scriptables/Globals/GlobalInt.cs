using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Global/Int", order = 2)]
public class GlobalInt : GlobalVariable<int> {}

[Serializable]
public class GlobalIntRef : GlobalRef<GlobalInt, int> {}



}
