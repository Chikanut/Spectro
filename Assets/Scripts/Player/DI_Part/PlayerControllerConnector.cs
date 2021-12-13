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
        
        View.SetDirectionalInput(signal.DirectionalInput);
    }
    
}
