using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {

[CreateAssetMenu(menuName = "Scriptables/Triggers/Basic Trigger", order = 100)]
public class SettableTrigger : ScriptableTrigger {
    public void Set() {
        _set = true;
    }

    public void Unset() {
        _set = false;
    }
}

}

