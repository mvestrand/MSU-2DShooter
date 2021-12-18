using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

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

[CustomPropertyDrawer(typeof(ScriptableReference<,>), true)]
public class ScriptableReferenceDrawer : PropertyDrawer {

    private static string[] options = {"Constant", "Variable"};

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        // Draw property label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var isConstRect = new Rect(position.x, position.y, position.height, position.height);
        var valueRect = new Rect(position.x + position.height + 2, position.y, position.width - (position.height + 2), position.height);

        var useConstant = property.FindPropertyRelative("useConstant");

        int index = (useConstant.boolValue ? 0 : 1);
        index = EditorGUI.Popup(isConstRect, index, options);

        useConstant.boolValue = (index == 0);


        if (useConstant.boolValue)
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("constantValue"), GUIContent.none);
        else
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("variable"), GUIContent.none);

        EditorGUI.EndProperty();
    }

}


[CustomEditor(typeof(ScriptableVariable<>),true)]
public class ScriptableVariableEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var targetVar = (IScriptableVariableCallback)target;

        if (GUILayout.Button("Update")) {
            targetVar.UpdateListeners();
        }
    }
}

}
