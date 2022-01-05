using System;
using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest {
    
[CreateAssetMenu(menuName="Global/Object Hook", order = 7)]
public sealed class ObjectHook : ScriptableObject {
    private bool setLock = false;
    private event Action<ObjectHook> onChange;
    [SerializeField] private UnityEngine.Object _value;
    [SerializeField] private Color _color;
    public Color Color { get { return _color; } }

    public T Get<T>() where T : UnityEngine.Object {
        if (_value == null)
            return null;
        else if (_value is T) {
            return (T)_value;
        } else {
            Debug.LogWarningFormat("Object hook \"{0}\" set to: ({1}) cannot be converted to type {2}", name, _value.FullName(), typeof(T).FullName);   
            return null;
        }
    }

    public void Attach<T>(T target) where T : UnityEngine.Object {
        if (_value != null && target != null && target != _value) {
            Debug.LogWarningFormat("Multiple objects attempting to attach themselves to hook \"{0}\":\n\told: ({1})\n\tnew: ({2})", name, _value.FullName(), target.FullName());
        } else if (target == _value) {
            Debug.LogWarningFormat("Object ({1}) attempting to attach itself to hook \"{0}\" multiple times", name, _value.FullName());
        }
        _value = target; 
        UpdateListeners(); 
    }

    public void Detach<T>(T target) where T : UnityEngine.Object {
        if (_value == target) {
            _value = null;
        } else if (_value != null) {
            Debug.LogWarningFormat("Object ({0}) attempting to detach from hook \"{1}\" which is owned by ({2})", target.FullName(), name, _value.FullName());
        } else {
            Debug.LogWarningFormat("Object ({0}) attempting to detach from hook \"{1}\" which is currently null", target.FullName(), name);
        }
    }


    public void Register(Action<ObjectHook> listener) {
        onChange += listener;
    }

    public void Deregister(Action<ObjectHook> listener) {
        onChange -= listener;        
    }

    public void UpdateListeners() {
        if (setLock) {
            Debug.LogError("Cyclical set call of var \""+name+"\": \n"+Environment.StackTrace);
            return;
        }
        setLock = true;
        onChange?.Invoke(this);
        setLock = false;
    }

}

[Serializable][InlineProperty]
public struct ObjectHookRef<T> where T : UnityEngine.Object {
    [SerializeField][HideLabel] private ObjectHook hook;
    public Color Color { get { return (hook != null ? hook.Color : Color.white); } }

    public bool TryGet(out T target) {
        target = null;
        if (hook != null) {
            target = hook.Get<T>();
        }
        return target != null;
    }

    public T Get() {
        return (hook != null ? hook.Get<T>() : null);
    }

    public void Register(Action<ObjectHook> listener) {
        hook?.Register(listener);
    }

    public void Deregister(Action<ObjectHook> listener) {
        hook?.Deregister(listener);
    }
}


[Serializable][InlineProperty]
public struct ObjectHookSetter<T> where T : UnityEngine.Object {
    [SerializeField][HideLabel] private ObjectHook hook;
    public Color Color { get { return (hook != null ? hook.Color : Color.gray); } }

    public void Attach(T target) {
        hook?.Attach<T>(target);
    }

    public void Detach(T target) {
        hook?.Detach<T>(target);
    }

    public bool TryGet(out T target) {
        target = null;
        if (hook != null) {
            target = hook.Get<T>();
        }
        return target != null;
    }

    public void Register(Action<ObjectHook> listener) {
        hook?.Register(listener);
    }

    public void Deregister(Action<ObjectHook> listener) {
        hook?.Deregister(listener);
    }


}

}
