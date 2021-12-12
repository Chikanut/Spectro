using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using UnityEngine;

namespace Player.InputSystem
{
    public class InputSignal : Signal
    {
        public Vector2 DirectionalInput;
    }

    
    public class PlayerInputConnector : Mediator<PlayerInput>
    {
        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

            View.Init(OnInputAction);
        }

        void OnInputAction(Vector2 dir)
        {
            SignalService.Publish(new InputSignal()
            {
                DirectionalInput = dir
            });
        }

    }
}
