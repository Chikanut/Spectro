using ShootCommon.GlobalStateMachine;

namespace Packages.Common.StateMachineGlobal.States
{
    public class InitState : GlobalState
    {
        protected override void Configure()
        {
            // Permit<StartState>(StateMachineTriggers.Start);
        }
    }
}