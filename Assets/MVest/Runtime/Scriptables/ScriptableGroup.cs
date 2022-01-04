using System;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest {

    public class ScriptableGroup<T> : ScriptableObject
    {
        private event Action<T> onAdd;
        private event Action<T> onRemove;
        [SerializeField] private List<T> _values = new List<T>();
        public List<T> Values
        {
            get { return _values; }
        }

        public bool Contains(T obj)
        {
            return _values.BinarySearch(obj) > 0;
        }

        public void Add(T obj)
        {
            var index = _values.BinarySearch(obj);
            if (index < 0) index = ~index;
            _values.Insert(index, obj);
            onAdd?.Invoke(obj);
        }

        public void Remove(T obj)
        {
            _values.Remove(obj);
            onRemove?.Invoke(obj);
        }

        public void OnValidate()
        {
            if (_values == null) _values = new List<T>();
            _values.Sort();
        }


        public void Register(Action<T> onAdd, Action<T> onRemove)
        {
            if (onAdd != null)
                this.onAdd += onAdd;
            if (onRemove != null)
                this.onRemove += onRemove;
        }

        public void Deregister(Action<T> onAdd, Action<T> onRemove)
        {
            if (onAdd != null)
                this.onAdd -= onAdd;
            if (onRemove != null)
                this.onRemove -= onRemove;
        }
    }
}
