using System.Collections;
using System.Collections.Generic;
using Player.InputSystem;
using ShootCommon.Views.Mediation;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

public class PlayerControllerConnector : Mediator<PlayerController>
{
    protected override void OnMediatorInitialize()
    {
        base.OnMediatorInitialize();
        
        InitializeEvents();

    }

    void InitializeEvents()
    {
        SignalService.Receive<InputSignal>().Subscribe(OnInputEvent).AddTo(DisposeOnDestroy);
    }

    void OnInputEvent(InputSignal signal)
    {
        var input = signal.DirectionalInput;
        
        if (Mathf.Abs(input.x) > 1)
            View.SetDirectionalInput(new Vector2(Mathf.Sign(input.x), 0));

        if(signal.DirectionalInput.y > 1)
            View.OnJump();
    }
    
}
