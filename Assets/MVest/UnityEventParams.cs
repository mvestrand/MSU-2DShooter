using UnityEngine.Events;
using UnityEngine;

namespace MVest {
#region Base C# Types
// Names use the .NET type names except where Unity does otherwise
[System.Serializable] public class UnityBooleanEvent : UnityEvent<bool> {}
[System.Serializable] public class UnityByteEvent : UnityEvent<byte> {}
[System.Serializable] public class UnitySByteEvent : UnityEvent<sbyte> {}
[System.Serializable] public class UnityCharEvent : UnityEvent<char> {}
[System.Serializable] public class UnityDecimalEvent : UnityEvent<decimal> {}
[System.Serializable] public class UnityDoubleEvent : UnityEvent<double> {}
[System.Serializable] public class UnityFloatEvent : UnityEvent<float> {}
[System.Serializable] public class UnityIntegerEvent : UnityEvent<int> {}
[System.Serializable] public class UnityUInt32Event : UnityEvent<uint> {}
//#if (C# Version >= 9.0)
// [System.Serializable] public class UnityIntPtrEvent : UnityEvent<nint> {}
// [System.Serializable] public class UnityUIntPtrEvent : UnityEvent<nuint> {}
//#endif
[System.Serializable] public class UnityInt64Event : UnityEvent<long> {}
[System.Serializable] public class UnityUInt64Event : UnityEvent<ulong> {}
[System.Serializable] public class UnityInt16Event : UnityEvent<short> {}
[System.Serializable] public class UnityUInt16Event : UnityEvent<ushort> {}
[System.Serializable] public class UnityStringEvent : UnityEvent<string> {}
[System.Serializable] public class UnityObjectEvent : UnityEvent<object> {}
#endregion

#region Unity Types
[System.Serializable] public class UnityUObjectEvent : UnityEvent<Object> {}
[System.Serializable] public class UnityGameObjectEvent : UnityEvent<GameObject> {}
[System.Serializable] public class UnityVector2Event : UnityEvent<Vector2> {}
[System.Serializable] public class UnityVector2IntEvent : UnityEvent<Vector2Int> {}
[System.Serializable] public class UnityVector3Event : UnityEvent<Vector3> {}
[System.Serializable] public class UnityVector3IntEvent : UnityEvent<Vector3Int> {}
[System.Serializable] public class UnityVector4Event : UnityEvent<Vector4> {}
#endregion
}
