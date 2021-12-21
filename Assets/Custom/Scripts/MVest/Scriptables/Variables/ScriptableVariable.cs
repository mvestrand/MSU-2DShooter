using System;
using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest {
public interface IScriptableVariable<T> {
    T Value {
        get;
        set;
    }

    void Register(Action<T> listener);
    void Deregister(Action<T> listener);
}

public interface IScriptableVariableCallback {
    void UpdateListeners();
}

public class ScriptableVariable<T> : ScriptableObject, IScriptableVariable<T>, IScriptableVariableCallback {
    private bool setLock = false;
    private event Action<T> onChange;
    [SerializeField] private T _value;
    public T Value {
        get { return _value; }
        set { _value = value; UpdateListeners(); }
    }

    public void Register(Action<T> listener) {
        onChange += listener;
    }

    public void Deregister(Action<T> listener) {
        onChange -= listener;        
    }

    public void UpdateListeners() {
        if (setLock) {
            Debug.LogError("Cyclical set call of var \""+name+"\": \n"+Environment.StackTrace);
            return;
        }
        setLock = true;
        onChange?.Invoke(_value);
        setLock = false;
    }
}

public interface IReadOnlyScriptableReference<TVar, T> where TVar : IScriptableVariable<T> {
    bool UseConstant { get; }
    T ConstantValue { get; }
    TVar Variable { get; }
    T Value { get; }
}

[Serializable][DrawWithUnity]
public class ScriptableReference<TVar, T> : IReadOnlyScriptableReference<TVar, T> where TVar : IScriptableVariable<T> {
    [SerializeField] private bool useConstant = true;
    [SerializeField] private T constantValue;
    [SerializeField] private TVar variable;
    public bool UseConstant { get { return useConstant; } set { useConstant = value; } }
    public T ConstantValue { get { return constantValue; } set { constantValue = value; } }
    public TVar Variable { get {return variable;} set {variable = value;} }

    public T Value {
        get { return useConstant ? constantValue : variable.Value; }
        set { if (UseConstant) constantValue = value; else variable.Value = value; }
    }
}

}
