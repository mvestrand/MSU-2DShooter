using System;
using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest
{

    public class ScriptableTrigger : ScriptableObject
    {
        [ShowInInspector] protected bool _set = false;

        protected virtual void Awake() {
            _set = OnGetStartState();
        }


        public bool Peek() { return _set; }

        public bool Get() {
            if (!_set)
                return false;
            else {
                return OnGetTrue();
            }
        }

        protected virtual bool OnGetTrue() {
            _set = false;
            return true;
        }

        protected virtual bool OnGetStartState() {
            return false;
        }

        public static implicit operator bool(ScriptableTrigger t) => t.Get();
    }
}
