using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using UnityEngine;

public class SomeSignal : Signal
{
        
}

namespace Player.InputSystem
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerInputView : View
    {
        private CharacterController _player;

        private void Start()
        {
            _player = GetComponent<CharacterController>();
        }

        void Update()
        {
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _player.SetDirectionalInput(directionalInput);

            if (Input.GetKeyDown(KeyCode.Space))
                _player.OnJump();
        }
    }
}
