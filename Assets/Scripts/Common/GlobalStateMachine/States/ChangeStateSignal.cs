using ShootCommon.Signals;

namespace ShootCommon.GlobalStateMachine.States
{
    public class ChangeStateSignal : Signal
    {
        public StateMachineTriggers SelectedState;
    }
}