using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequireInterfaceAttribute : PropertyAttribute {

    public System.Type requiredType { get; private set; }

    public RequireInterfaceAttribute(System.Type requiredType) {
        this.requiredType = requiredType;
    }

}
