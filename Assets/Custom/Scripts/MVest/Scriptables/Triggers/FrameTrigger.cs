using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {

[CreateAssetMenu(menuName = "Scriptables/Triggers/Frame Trigger", order = 100)]
public class FrameTrigger : ScriptableTrigger {

    protected override bool OnGetTrue() {
        _set = false;
        return true;
    }

    protected override bool OnGetStartState() {
        return true;
    }

}

}

