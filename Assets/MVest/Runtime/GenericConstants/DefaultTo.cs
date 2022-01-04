using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest {

    [System.Serializable][InlineProperty]
    public struct DefaultTo<T,C> where C : struct, IConstant<T> {
        private static T defaultValue;

        static DefaultTo() {
            C constInstance = new C();
            constInstance.Value(out defaultValue);
        }

        [SerializeField][HideLabel][HorizontalGroup("defaultTo", 10)] private bool _set;
        public bool IsSet { get { return _set; } set { _set = value; } }
        [EnableIf("_set")][SerializeField][HideLabel][HorizontalGroup("defaultTo")] private T _value;
        public T Value {
            get { return (_set ? _value : defaultValue); }
            set { _value = value; _set = true; }
        }

        public DefaultTo(T value) {
            _value = value;
            _set = true;
        }

        public void Unset() { _set = false; }

        public static implicit operator T(DefaultTo<T,C> d) => (d._set ? d._value : defaultValue);
        public static implicit operator DefaultTo<T,C>(T t) => (new DefaultTo<T,C>(t));
    }



}
