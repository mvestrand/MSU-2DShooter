using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(menuName="Custom/Element", fileName ="NewElement", order = 0)]
public class Element : SerializedScriptableObject
{
    [SerializeField] private string _elementName;
    public string ElementName { get { return _elementName; } }
    [SerializeField] private Color _baseColor;
    public Color BaseColor { get { return _baseColor; } }
    [SerializeField] private float DamageToUntyped = 1f;
    [SerializeField] private float DamageFromUntyped = 1f;
    public Dictionary<Element, float> DamageToMultiplier = new Dictionary<Element, float>();


    /// <summary>
    /// Computes the damage multiplier for a damage source versus a given target element.
    /// </summary>
    /// <param name="source"> Element of the damage source, use null for untyped. </param>
    /// <param name="target"> Element of the target of the damage, use null for untyped.</param>
    /// <returns> Damage multiplier for the given element combination. </returns>
    public static float DamageMultiplier(Element source, Element target) {
        float multiplier = 1f;
        // Untyped => Untyped
        if (source == null && target == null) 
            return 1f;
        // Untyped => Typed
        else if (source == null) 
            return target.DamageFromUntyped;
        // Typed => Untyped
        else if (target == null)
            return source.DamageToUntyped;
        // Typed => Typed
        else if (source.DamageToMultiplier.TryGetValue(target, out multiplier)) 
            return multiplier;
        // Unplanned for type combination
        else { 
            Debug.LogWarningFormat("Unknown type combination: ({0}) => ({1})", source.name, target.name);
            return 1f;
        }
    }
}

/// <summary>
/// Element extension methods to make dealing with null values smoother
/// </summary>
public static class ElementExtensions {
    public static float DamageTo(this Element source, Element target) {
        return Element.DamageMultiplier(source, target);
    }

    public static bool IsUntyped(this Element element) {
        return element == null;
    }

    public static string ToString(this Element element) {
        return (element != null ? element.ElementName : "Untyped");
    }

    public static Color GetColor(this Element element) {
        return (element != null ? element.BaseColor : Color.gray);
    }
} 
