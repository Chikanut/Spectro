using System;
using System.Collections.Generic;
using Packages.Common.StateMachineGlobal.States;
using Stateless;
using UnityEngine;
using Zenject;

namespace ShootCommon.GlobalStateMachine
{
    public class StateMachineController :StateMachine<IState, StateMachineTriggers>, IStateMachineController, IInitializable
    {
        private StateMachine<IState, StateMachineTriggers> _stateMachine =
            new StateMachine<IState, StateMachineTriggers>(null);
        
        private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();

        private StateMachineTriggers _currentState;
        public StateMachineTriggers CurrentState => _currentState;

        public StateMachineController(InitState initialState) : base(initialState)
        {
            Debug.Log("tt");
        }

        [Inject]
        public void Init(IState[] states)
        {
            Debug.Log("StateMachineController Init");
            for (var i = 0; i != states.Length; i++)
            {
                var state = states[i];
                _states.Add(state.GetType(), state);
            }
        }
        
        public void Initialize()
        {
            Debug.Log("StateMachineController Initialize");
            Fire(StateMachineTriggers.Start);
        }
        
        public IState GetState<TState>()
            where TState: IState
        {
            if (_states.TryGetValue(typeof(TState), out var state))
                return state;
            
            throw new InvalidOperationException($"No state of type {typeof(TState).Name} binded");
        }

        public void FireState(StateMachineTriggers triggers)
        {
            _currentState = triggers;
            Fire(triggers);
        }
    }
}