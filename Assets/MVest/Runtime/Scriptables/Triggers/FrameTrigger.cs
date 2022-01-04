using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {



    /// <summary>
    /// A trigger that returns true on the first call of it to each frame and false otherwise
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptables/Triggers/Frame Trigger", order = 100)]
    public class FrameTrigger : ScriptableTrigger {

        private static WaitForEndOfFrame waitInstruction = new WaitForEndOfFrame();

        // private int lastFrame = -1;

        protected override bool OnGetTrue() {
            _set = false;
            return true;
        }


        protected override bool OnGetStartState() {
            return true;
        }
    }

}

