using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

[System.Serializable]
[CreateAssetMenu(menuName="Custom/ElementModifier", fileName ="NewElementModifier", order = 1)]
public class ElementModifier : SerializedScriptableObject
{

    [System.NonSerialized][OdinSerialize]
    [ListDrawerSettings(DraggableItems=false, ShowIndexLabels=true, ShowItemCount=false, HideRemoveButton=true)] 
    public ElementModifierData[] Modifiers;

    public ElementModifier() {
        Modifiers = new ElementModifierData[GameManager.DifficultyOptionsCount()];
        for (int i = 0; i < Modifiers.Length; i++) {
            Modifiers[i] = new ElementModifierData();
            Modifiers[i].defaultModifier = 1f;
            Modifiers[i].Modifier = new Dictionary<Element, float>();
        }
    }

    public float GetModifier(Element element) {
        return Modifiers[(int)GameManager.Difficulty].GetModifier(element);
    }

}

[System.Serializable]
public struct ElementModifierData {

    [Tooltip("The fallback modifier value when an element combination is unknown")]
    public float defaultModifier;
    public Dictionary<Element, float> Modifier;
    public float GetModifier(Element element) {
        if (element == null)
            return defaultModifier;
        float modifier;
        if (Modifier.TryGetValue(element, out modifier))
            return modifier;
        return defaultModifier;
    }

}

public static class ElementModifierExtensions {

    public static float Modify(this ElementModifier modifier, Element element) {
        return (modifier != null ? modifier.GetModifier(element) : 1f);
    }
}