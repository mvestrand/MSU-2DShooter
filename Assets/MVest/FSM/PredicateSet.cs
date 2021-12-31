using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {

public abstract class PredicateSet<TShared> : ScriptableObject {

    protected delegate bool Predicate(TShared sharedData, StateMachine<TShared> fsm, int predIndex);
    protected Predicate[] predicates;
    public bool TestPredicate(int predicateIndex, TShared sharedData, StateMachine<TShared> fsm) {
        if (predicates[predicateIndex] == null) {
            Debug.LogErrorFormat("Error in state machine {0}[{1}], predicate set {2} has removed predicate {3}", 
                                    fsm.Template.name, fsm.CurrentState.name, this.name, predicateIndex);
            return false;
        }
        return predicates[predicateIndex](sharedData, fsm, predicateIndex);
    }
}

}