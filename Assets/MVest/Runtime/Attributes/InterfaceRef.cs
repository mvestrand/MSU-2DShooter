using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest {
[System.Serializable][InlineProperty]
public struct InterfaceRef<TInterface> where TInterface : class {
    [SerializeField][HideLabel] private Object _value;
    public TInterface i {
        get { return _value as TInterface; }
        set { _value = value as Object; }
    }
}

}

