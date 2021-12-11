using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootCommon.GlobalStateMachine
{
    public abstract class Substate<TState> : GlobalState
        where TState : IState
    {
        protected override void Configure()
        {
            SubstateOf<TState>();
        }
    }
}