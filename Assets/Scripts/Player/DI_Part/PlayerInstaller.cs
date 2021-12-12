using Player.InputSystem;
using ShootCommon.Views.Mediation;
using Zenject;

namespace Player
{
    public class PlayerInstaller : Installer<PlayerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindViewToMediator<PlayerInput, PlayerInputConnector>();
            Container.BindViewToMediator<PlayerController, PlayerControllerConnector>();
        }
    }
}
