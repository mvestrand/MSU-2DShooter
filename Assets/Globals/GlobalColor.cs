using System;
using UnityEngine;

using MVest.Unity.Globals;

/// <summary>
/// A global variable of type System.Single
/// </summary>
[CreateAssetMenu(menuName = "Global/Color", order = 3)]
public class GlobalColor : GlobalVariable<Color> { }

/// <summary>
/// A global variable reference of type System.Single
/// </summary>
[Serializable]
public class GlobalColorRef : GlobalRef<GlobalColor, Color> { }
