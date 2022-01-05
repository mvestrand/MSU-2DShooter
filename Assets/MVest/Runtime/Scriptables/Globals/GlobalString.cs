using System;
using UnityEngine;


namespace MVest {

[CreateAssetMenu(menuName="Global/String", order = 4)]
public class GlobalString : GlobalVariable<string> {}

[Serializable]
public class GlobalStringRef : GlobalRef<GlobalString, string> {}

}
