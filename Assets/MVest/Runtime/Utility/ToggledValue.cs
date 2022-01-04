using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// TODO I think that instead of using DefaultTo I can just use something like this and a custom 
//  property drawer to give a variable a togglable variable a default value
public struct ToggledValue<T> {
    [SerializeField] private T _defaultValue;
    [SerializeField] private bool _isSet;
    [SerializeField] private T _value;


}
