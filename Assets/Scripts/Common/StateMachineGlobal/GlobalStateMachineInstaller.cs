using Packages.Common.StateMachineGlobal.States;
using ShootCommon.GlobalStateMachine;
using Zenject;

namespace Packages.Common.StateMachineGlobal
{
    public class GlobalStateMachineInstaller : Installer<GlobalStateMachineInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindState<InitState>();

            //Bind states inside
            
            Container.BindInterfacesTo<StateMachineController>().AsSingle();
        }
    }
}