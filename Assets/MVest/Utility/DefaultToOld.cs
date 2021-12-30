
using System;
using System.Reflection;

using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest {

    [System.Serializable][InlineProperty]
    public struct DefaultToOld<T,C> where C : MVest.ConstOld.ConstantOld, new() {
        private static T defaultValue;

        static DefaultToOld() {
            C constInstance = new C();
            defaultValue = constInstance.Value<T>();
        }

        [HorizontalGroup(10)]
        [HideLabel]
        [SerializeField] private bool _set;
        public bool IsSet { get { return _set; } set { _set = value; } }
        [HorizontalGroup]
        [HideLabel]
        [EnableIf("_set")][SerializeField] private T _value;
        public T Value {
            get { return (_set ? _value : defaultValue); }
            set { _value = value; _set = true; }
        }        

        public DefaultToOld(T value) {
            _value = value;
            _set = true;
        }

        public void Unset() { _set = false; }

        public static implicit operator T(DefaultToOld<T,C> d) => (d._set ? d._value : defaultValue);
        public static implicit operator DefaultToOld<T,C>(T t) => (new DefaultToOld<T,C>(t));
    }
}


