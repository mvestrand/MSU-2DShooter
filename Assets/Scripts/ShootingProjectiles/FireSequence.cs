using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FireSequenceData {
    public float time;
    public BulletPattern pattern;
    public int bindingNumber;

}

[CreateAssetMenu(menuName = "Fire Sequence")]
public class FireSequence : ScriptableObject {

}
