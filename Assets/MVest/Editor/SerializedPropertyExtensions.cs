using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public static class SerializedPropertyExtensionMethods {
    public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property) {
        var currentProperty = property.Copy();
        var nextSiblingProperty = property.Copy();
        nextSiblingProperty.Next(false);

        if (currentProperty.Next(true)) {
            do {
                if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    break;
                yield return currentProperty.Copy();
            } 
            while (currentProperty.Next(false));
        }
    }

    public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty property) {
        var currentProperty = property.Copy();
        var nextSiblingProperty = property.Copy();
        nextSiblingProperty.NextVisible(false);

        if (currentProperty.NextVisible(true)) {
            do {
                if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    break;
                yield return currentProperty.Copy();
            } 
            while (currentProperty.NextVisible(false));
        }

    }
}