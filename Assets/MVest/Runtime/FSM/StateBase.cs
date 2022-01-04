using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {
    public abstract class StateBase<TShared> : ScriptableObject {
        public abstract object GetData(TShared sharedData);
        public abstract void ReleaseData(object stateData);
        public abstract bool TestPredicate(int predicateIndex, object stateData, TShared sharedData, StateMachine<TShared> fsm);
        public abstract string[] GetPredicates();

        public abstract void Tick(object stateData, TShared sharedData);
        public abstract void OnEnter(object stateData, TShared sharedData);
        public abstract void OnExit(object stateData, TShared sharedData);
    }

}
