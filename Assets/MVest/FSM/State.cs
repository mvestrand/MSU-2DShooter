using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {
    public abstract class State<TShared> : StateBase<TShared> {
        public override object GetData(TShared sharedData) {return null;}
        public override void ReleaseData(object stateData) {}

        
        protected delegate bool Predicate(TShared sharedData, StateMachine<TShared> fsm);
        private string[] predicateNames = null;
        private Predicate[] predicates;
        public override bool TestPredicate(int predicateIndex, object stateData, TShared sharedData, StateMachine<TShared> fsm) {
            if (predicates[predicateIndex] == null) {
                Debug.LogErrorFormat("State machine transition {0}[{1}] Predicate index {2} is null predicate", 
                                        fsm.Template.name, fsm.CurrentState.name, predicateIndex);
                return false;
            }
            return predicates[predicateIndex](sharedData, fsm);
        }
        public override string[] GetPredicates() {
            return predicateNames;
        }

        protected virtual void Awake() { 
            predicates = this.DefinePredicates();
            string[] predicateNames = new string[predicates.Length];
            for (int i = 0; i < predicateNames.Length; i++) {
                predicateNames[i] = predicates[i].Method.Name;
            }
        }
        protected virtual Predicate[] DefinePredicates() {return new Predicate[0];}
        public override void Tick(object stateData, TShared sharedData) { this.Tick(sharedData);}
        public override void OnEnter(object stateData, TShared sharedData) {this.OnEnter(sharedData);}
        public override void OnExit(object stateData, TShared sharedData) {this.OnExit(sharedData);}
        public abstract void Tick(TShared sharedData);
        public abstract void OnEnter(TShared sharedData);
        public abstract void OnExit(TShared sharedData);
    }

}
