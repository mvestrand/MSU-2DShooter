using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace MVest {

    public class StateMachineTemplate<TShared> : SerializedScriptableObject {

        [SerializeField]
        internal Dictionary< StateBase<TShared>, List<StateMachineTransition<TShared>> > _transitions = 
                    new Dictionary< StateBase<TShared>, List<StateMachineTransition<TShared>> >();
        [SerializeField] internal List<StateMachineTransition<TShared>> _anyTransitions = new List<StateMachineTransition<TShared>>();
        [SerializeField] internal List<StateMachineTransition<TShared>> _emptyTransitions = new List<StateMachineTransition<TShared>>();
        [SerializeField] protected StateBase<TShared> _defaultState;
        public StateBase<TShared> DefaultState { get { return _defaultState; } }

        public StateMachine<TShared> CreateStateMachine_NullState() {
            return new StateMachine<TShared>(this);
        }

        public StateMachine<TShared> CreateStateMachine(TShared sharedData) {
            return new StateMachine<TShared>(this, _defaultState, sharedData);
        }

        public StateMachine<TShared> CreateStateMachine(TShared sharedData, StateBase<TShared> entryState) {
            return new StateMachine<TShared>(this, entryState, sharedData);
        }

    }

    public class StateMachine<TShared> {
        private StateMachineTemplate<TShared> _template;
        public StateMachineTemplate<TShared> Template { get { return _template; } }
        private StateBase<TShared> _currentState;
        public StateBase<TShared> CurrentState { get { return _currentState; } }
        private object _currentStateData;
        private List<StateMachineTransition<TShared>> _currentTransitions;

        public StateMachine(StateMachineTemplate<TShared> template) {
            _template = template;
            _currentState = null;
            _currentStateData = null;
            _currentTransitions = template._emptyTransitions;
        }

        public StateMachine(StateMachineTemplate<TShared> template, StateBase<TShared> entryState, TShared sharedData) {
            _template = template;
            SetState(entryState, sharedData);
        }

        public void Tick(TShared sharedData) {
            GetTransition(_currentStateData, sharedData, out var transition);
            if (transition != null)
                SetState(transition.to, sharedData);

            _currentState?.Tick(_currentStateData, sharedData);
        }

        public void SetState(StateBase<TShared> newState, TShared sharedData) {
            if (newState == _currentState) // Already in requested state
                return;

            // Cleanup the current state
            if (_currentState != null) {
                _currentState.OnExit(_currentStateData, sharedData);
                _currentState.ReleaseData(_currentStateData);
            }

            // Set and instantiate the new state
            _currentState = newState;
            if (_currentState != null)
                _currentStateData = _currentState.GetData(sharedData);

            // Get the new transitions or the default transitions
            if (!_template._transitions.TryGetValue(newState, out _currentTransitions)) {
                _currentTransitions = _template._emptyTransitions;
            }

            // Enter the new state
            _currentState?.OnEnter(_currentStateData, sharedData);
        }

        private void GetTransition(object stateData, TShared sharedData, out StateMachineTransition<TShared> transition) {
            foreach (var trans in _template._anyTransitions) {
                if (trans.predicateSet.TestPredicate(trans.predicateIndex, sharedData, this)) {
                    transition = trans;
                    return;
                }
            }

        foreach (var trans in _currentTransitions) {
                if (trans.predicateSet == null && _currentState.TestPredicate(trans.predicateIndex, stateData, sharedData, this)) {

                } else if (trans.predicateSet.TestPredicate(trans.predicateIndex, sharedData, this)) {
                    transition = trans;
                    return;
                }
            }
            transition = null;
        }
    }

    [System.Serializable]
    public class StateMachineTransition<TShared> {
        public StateBase<TShared> from;
        public StateBase<TShared> to;
        // public StateDataBase<TShared> initData;
        [Tooltip("The set of predicates to use. Set to null to query the current state instead (cannot be used for universal transitions).")]
        public PredicateSet<TShared> predicateSet;
        public int predicateIndex;
    }

}