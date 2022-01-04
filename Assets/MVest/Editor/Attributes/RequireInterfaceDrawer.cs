using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
public class RequireInterfaceDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType == SerializedPropertyType.ObjectReference) {
            var requiredAttribute = this.attribute as RequireInterfaceAttribute;
            
            EditorGUI.BeginProperty(position, label, property);

            var reference = EditorGUI.ObjectField(position, label, property.objectReferenceValue, requiredAttribute.requiredType, true);

            if (reference == null) {
                var obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Object), true);

                if (obj is GameObject g) {
                    reference = g.GetComponent(requiredAttribute.requiredType);
                }
            } 

            property.objectReferenceValue = reference;

            EditorGUI.EndProperty();
        } else {
            EditorGUILayout.HelpBox($"{property.name} is not an object reference. RequireInterface can only be used on object reference types", MessageType.Error);
            base.OnGUI(position, property, label);
        }
    }

}
