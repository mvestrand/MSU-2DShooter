using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Pool;

namespace MVest {

    [System.Serializable]
    public abstract class DataState<TData, TShared> : StateBase<TShared> where TData : class, IStateData<TShared>, new() {

        private ObjectPool<TData> dataObjPool;

        static TData CreateData() {
            return new TData();
        }

        public override object GetData(TShared sharedData) {
            var data = dataObjPool.Get();
            data.Reset(sharedData);
            return data;
        }
        public override void ReleaseData(object stateData) {
            dataObjPool.Release((TData)stateData);
        }

        protected delegate bool Predicate(TData stateData, TShared sharedData, StateMachine<TShared> fsm);
        private string[] predicateNames = null;
        private Predicate[] predicates;

        public override void Tick(object stateData, TShared sharedData) { this.Tick((TData)stateData, sharedData); }
        public override void OnEnter(object stateData, TShared sharedData) { this.OnEnter((TData)stateData, sharedData); }
        public override void OnExit(object stateData, TShared sharedData) { this.OnExit((TData)stateData, sharedData); }
        public override bool TestPredicate(int predicateIndex, object stateData, TShared sharedData, StateMachine<TShared> fsm) {
            if (predicates[predicateIndex] == null) {
                Debug.LogErrorFormat("Error in state machine transition {0}[{1}], has null predicate {2}", 
                                        fsm.Template.name, fsm.CurrentState.name, predicateIndex);
                return false;
            }
            return predicates[predicateIndex]((TData)stateData, sharedData, fsm);
        }
        public override string[] GetPredicates() {
            return predicateNames;
        }

        protected virtual void Awake() { 
            dataObjPool = new ObjectPool<TData>(CreateData);
            predicates = this.DefinePredicates();
            string[] predicateNames = new string[predicates.Length];
            for (int i = 0; i < predicateNames.Length; i++) {
                predicateNames[i] = (predicates[i] != null ? predicates[i].Method.Name : "--UNUSED--");
            }
        }


        protected virtual Predicate[] DefinePredicates() {return new Predicate[0];}
        protected abstract void Tick(TData stateData, TShared sharedData);
        protected abstract void OnEnter(TData stateData, TShared sharedData);
        protected abstract void OnExit(TData stateData, TShared sharedData);

    }

    public interface IStateData<TShared> {
        void Reset(TShared sharedData);
    }


}

